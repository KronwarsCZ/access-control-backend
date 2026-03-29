using ApiShared.Dto.IoT;
using Application.Services.AccessList;
using Application.Services.Provisioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccessControl.Web.Api.V1.IoT;

/// <summary>
/// Controller for access list (allowed UIDs) management.
/// These endpoints require mTLS authentication.
/// </summary>
[ApiController]
[Route("api/v1/locks")]
public class AccessListController : IoTControllerBase
{
    private readonly IAccessListService _accessListService;
    private readonly IProvisioningService _provisioningService;
    private readonly ILogger<AccessListController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AccessListController(
        IAccessListService accessListService,
        IProvisioningService provisioningService,
        ILogger<AccessListController> logger,
        IWebHostEnvironment environment)
    {
        _accessListService = accessListService;
        _provisioningService = provisioningService;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Returns the list of allowed RFID UIDs for a specific lock.
    /// Authentication: mTLS (client certificate required).
    /// Supports both binary and JSON formats via Accept header.
    /// </summary>
    /// <param name="deviceId">Device identifier (e.g., DEV-AABBCCDDEEFF)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary allowlist or JSON response based on Accept header</returns>
    [HttpGet("{deviceId}/allowed")]
    [ProducesResponseType(typeof(AllowedUidsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllowedUids(
        [FromRoute] string deviceId,
        CancellationToken cancellationToken)
    {
        var certificateValidationError = await ValidateCertificateAndProvisioningAsync(deviceId, cancellationToken);
        if (certificateValidationError != null)
            return certificateValidationError;

        // Validate device ID format
        if (!_accessListService.ValidateDeviceId(deviceId))
        {
            _logger.LogWarning("Invalid device ID format: {DeviceId}", deviceId);
            return BadRequest(new { error = "Invalid device ID format" });
        }

        try
        {
            // Check Accept header to determine response format
            var acceptHeader = Request.Headers.Accept.ToString();
            var preferBinary = acceptHeader.Contains("application/octet-stream") ||
                               !acceptHeader.Contains("application/json");

            if (preferBinary)
            {
                // Return binary format (recommended for ESP32)
                var binaryData = await _accessListService.GenerateBinaryAllowlistAsync(deviceId, cancellationToken);
                
                _logger.LogInformation("Returning binary allowlist ({Size} bytes) for device {DeviceId}", 
                    binaryData.Length, deviceId);
                
                return File(binaryData, "application/octet-stream");
            }
            else
            {
                // Return JSON format
                var jsonResponse = await _accessListService.GetAllowedUidsResponseAsync(deviceId, cancellationToken);
                
                _logger.LogInformation("Returning JSON allowlist ({Count} entries) for device {DeviceId}", 
                    jsonResponse.Entries.Count(), deviceId);
                
                return Ok(jsonResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access list for device {DeviceId}", deviceId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Error retrieving access list" });
        }
    }

    /// <summary>
    /// Returns the access list in JSON format explicitly.
    /// </summary>
    [HttpGet("{deviceId}/allowed.json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AllowedUidsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AllowedUidsResponse>> GetAllowedUidsJson(
        [FromRoute] string deviceId,
        CancellationToken cancellationToken)
    {
        var certificateValidationError = await ValidateCertificateAndProvisioningAsync(deviceId, cancellationToken);
        if (certificateValidationError != null)
            return certificateValidationError;

        if (!_accessListService.ValidateDeviceId(deviceId))
        {
            return BadRequest(new { error = "Invalid device ID format" });
        }

        try
        {
            var response = await _accessListService.GetAllowedUidsResponseAsync(deviceId, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving JSON access list for device {DeviceId}", deviceId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Error retrieving access list" });
        }
    }

    /// <summary>
    /// Returns the access list in binary format explicitly.
    /// </summary>
    [HttpGet("{deviceId}/allowed.bin")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllowedUidsBinary(
        [FromRoute] string deviceId,
        CancellationToken cancellationToken)
    {
        var certificateValidationError = await ValidateCertificateAndProvisioningAsync(deviceId, cancellationToken);
        if (certificateValidationError != null)
            return certificateValidationError;

        if (!_accessListService.ValidateDeviceId(deviceId))
        {
            return BadRequest(new { error = "Invalid device ID format" });
        }

        try
        {
            var binaryData = await _accessListService.GenerateBinaryAllowlistAsync(deviceId, cancellationToken);
            return File(binaryData, "application/octet-stream", $"{deviceId}_allowlist.bin");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving binary access list for device {DeviceId}", deviceId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Error retrieving access list" });
        }
    }

    private async Task<ActionResult?> ValidateCertificateAndProvisioningAsync(string deviceId, CancellationToken cancellationToken)
    {
        var clientCertificate = await GetClientCertificateAsync(_environment, _logger, cancellationToken);
        if (clientCertificate == null)
        {
            _logger.LogWarning("No client certificate provided for access list request");
            return Unauthorized(new { error = "Client certificate required" });
        }

        var clientCn = ExtractCnFromSubject(clientCertificate.Subject);
        if (!string.Equals(clientCn, deviceId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Device {ClientCn} attempted to access list for {DeviceId}", clientCn, deviceId);
            return StatusCode(StatusCodes.Status403Forbidden,
                new { error = "Device can only access its own allowlist" });
        }

        var isProvisioned = await _provisioningService.IsDeviceProvisionedAsync(deviceId, cancellationToken);
        if (!isProvisioned)
        {
            _logger.LogWarning("Unprovisioned or inactive device {DeviceId} requested allowlist", deviceId);
            return Unauthorized(new { error = "Device is not provisioned or is inactive" });
        }

        return null;
    }
}
