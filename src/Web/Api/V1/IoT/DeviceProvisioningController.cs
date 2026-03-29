using ApiShared.Dto.IoT;
using Application.Services.Provisioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccessControl.Web.Api.V1.IoT;

/// <summary>
/// Controller for IoT device provisioning and certificate renewal.
/// </summary>
[ApiController]
[Route("api/v1")]
public class DeviceProvisioningController : IoTControllerBase
{
    private readonly IProvisioningService _provisioningService;
    private readonly ILogger<DeviceProvisioningController> _logger;
    private readonly IWebHostEnvironment _environment;

    public DeviceProvisioningController(
        IProvisioningService provisioningService,
        ILogger<DeviceProvisioningController> logger,
        IWebHostEnvironment environment)
    {
        _provisioningService = provisioningService;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Provisions a new device by exchanging a CSR for a signed client certificate.
    /// Authentication: Shared secret in request body.
    /// </summary>
    /// <param name="request">Provisioning request with CSR and device info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Signed client certificate and CA certificate</returns>
    [HttpPost("provisioning")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProvisioningResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProvisioningResponse>> ProvisionDevice(
        [FromBody] ProvisioningRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(m => m.Value?.Errors.Count > 0)
                .ToDictionary(
                    m => m.Key,
                    m => m.Value!.Errors.Select(e => e.ErrorMessage + (e.Exception != null ? $" ({e.Exception.Message})" : "")).ToArray());
            _logger.LogWarning("Invalid provisioning request: {@ValidationErrors}", errors);
            return BadRequest(ModelState);
        }

        // Validate device ID format
        if (!_provisioningService.ValidateDeviceIdFormat(request.DeviceId))
        {
            _logger.LogWarning("Invalid device ID format: {DeviceId}", request.DeviceId);
            return BadRequest(new { error = "Invalid device ID format" });
        }

        try
        {
            var response = await _provisioningService.ProvisionDeviceAsync(request, cancellationToken);
            _logger.LogInformation("Successfully provisioned device {DeviceId}", request.DeviceId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized provisioning attempt for device {DeviceId}: {Message}", 
                request.DeviceId, ex.Message);
            return Unauthorized(new { error = "Invalid shared secret" });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already provisioned"))
        {
            _logger.LogWarning("Device already provisioned: {DeviceId}", request.DeviceId);
            return Conflict(new { error = "Device already provisioned" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid provisioning request for device {DeviceId}: {Message}", 
                request.DeviceId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error provisioning device {DeviceId}", request.DeviceId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Internal server error during provisioning" });
        }
    }

    /// <summary>
    /// Renews an expiring device certificate using mTLS authentication.
    /// Authentication: mTLS (client certificate required).
    /// </summary>
    /// <param name="request">Renewal request with new CSR</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New signed client certificate</returns>
    [HttpPost("renew")]
    [ProducesResponseType(typeof(RenewalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RenewalResponse>> RenewCertificate(
        [FromBody] RenewalRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid renewal request model state");
            return BadRequest(ModelState);
        }

        var clientCertificate = await GetClientCertificateAsync(_environment, _logger, cancellationToken);
        if (clientCertificate == null)
        {
            _logger.LogWarning("No client certificate provided for renewal");
            return Unauthorized(new { error = "Client certificate required" });
        }

        // Extract CN from the subject
        var clientCn = ExtractCnFromSubject(clientCertificate.Subject);
        if (string.IsNullOrEmpty(clientCn))
        {
            _logger.LogWarning("Could not extract CN from client certificate");
            return Unauthorized(new { error = "Invalid client certificate" });
        }

        if (!_provisioningService.ValidateDeviceIdFormat(request.DeviceId))
        {
            _logger.LogWarning("Invalid renewal device ID format: {DeviceId}", request.DeviceId);
            return BadRequest(new { error = "Invalid device ID format" });
        }

        var isProvisioned = await _provisioningService.IsDeviceProvisionedAsync(request.DeviceId, cancellationToken);
        if (!isProvisioned)
        {
            _logger.LogWarning("Renewal attempted for unprovisioned/inactive device {DeviceId}", request.DeviceId);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "Device not provisioned" });
        }

        try
        {
            var response = await _provisioningService.RenewCertificateAsync(request, clientCn, cancellationToken);
            _logger.LogInformation("Successfully renewed certificate for device {DeviceId}", request.DeviceId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized renewal attempt for device {DeviceId}: {Message}", 
                request.DeviceId, ex.Message);
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Forbidden renewal attempt for device {DeviceId}: {Message}", 
                request.DeviceId, ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid renewal request for device {DeviceId}: {Message}", 
                request.DeviceId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renewing certificate for device {DeviceId}", request.DeviceId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Internal server error during renewal" });
        }
    }

}
