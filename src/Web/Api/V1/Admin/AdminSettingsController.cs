using System.Security.Claims;
using ApiShared.Dto.Admin;
using Application.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1.Admin;

/// <summary>
/// Controller for admin settings management (OTA, provisioning).
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public class AdminSettingsController : ControllerBase
{
    private readonly IAdminSettingsService _adminSettingsService;
    private readonly ILogger<AdminSettingsController> _logger;

    public AdminSettingsController(
        IAdminSettingsService adminSettingsService,
        ILogger<AdminSettingsController> logger)
    {
        _adminSettingsService = adminSettingsService;
        _logger = logger;
    }

    private Guid GetOwnerIdFromClaims()
    {
        var claim = User.FindFirstValue("OwnerId");
        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var ownerId))
            throw new UnauthorizedAccessException("OwnerId claim is missing or invalid");
        return ownerId;
    }

    #region OTA Settings (per-owner)

    [HttpGet("ota/settings")]
    [ProducesResponseType(typeof(OtaSettingsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OtaSettingsResponse>> GetOtaSettings(CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerIdFromClaims();
        var settings = await _adminSettingsService.GetOtaSettingsAsync(ownerId, cancellationToken);
        return Ok(settings);
    }

    [HttpPost("ota/version")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateFirmwareVersion(
        [FromBody] UpdateOtaSettingsRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = GetOwnerIdFromClaims();
        await _adminSettingsService.UpdateFirmwareVersionAsync(ownerId, request.FirmwareVersion, cancellationToken);
        return Ok(new { message = "Firmware version updated" });
    }

    [HttpPost("ota/upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadFirmware(
        IFormFile firmware,
        [FromForm] string version,
        CancellationToken cancellationToken)
    {
        if (firmware == null || firmware.Length == 0)
            return BadRequest(new { error = "No firmware file provided" });

        if (string.IsNullOrWhiteSpace(version))
            return BadRequest(new { error = "Version is required" });

        if (!System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"))
            return BadRequest(new { error = "Version must be in format X.Y.Z" });

        var ownerId = GetOwnerIdFromClaims();

        try
        {
            await using var stream = firmware.OpenReadStream();
            await _adminSettingsService.SaveFirmwareAsync(ownerId, stream, version, cancellationToken);

            _logger.LogInformation(
                "Firmware uploaded for owner {OwnerId}: {FileName}, Version: {Version}, Size: {Size} bytes",
                ownerId, firmware.FileName, version, firmware.Length);

            return Ok(new { message = "Firmware uploaded successfully", version, size = firmware.Length });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading firmware for owner {OwnerId}", ownerId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error uploading firmware" });
        }
    }

    [HttpPost("ota/enabled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetOtaEnabled(
        [FromBody] UpdateOtaEnabledRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerIdFromClaims();
        try
        {
            await _adminSettingsService.SetOtaEnabledAsync(ownerId, request.Enabled, cancellationToken);
            return Ok(new { message = request.Enabled ? "OTA enabled" : "OTA disabled" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Provisioning Settings (per-owner)

    /// <summary>
    /// Gets current provisioning settings for the calling owner.
    /// </summary>
    [HttpGet("provisioning/settings")]
    [ProducesResponseType(typeof(ProvisioningSettingsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProvisioningSettingsResponse>> GetProvisioningSettings(CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerIdFromClaims();
        var settings = await _adminSettingsService.GetProvisioningSettingsAsync(ownerId, cancellationToken);
        return Ok(settings);
    }

    /// <summary>
    /// Sets the provisioning secret for the calling owner.
    /// </summary>
    [HttpPost("provisioning/secret")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProvisioningSecret(
        [FromBody] UpdateProvisioningSecretRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var ownerId = GetOwnerIdFromClaims();
            await _adminSettingsService.UpdateProvisioningSecretAsync(ownerId, request.NewSecret, request.Label, cancellationToken);
            return Ok(new { message = "Provisioning secret updated" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Clears the provisioning secret for the calling owner.
    /// </summary>
    [HttpDelete("provisioning/secret")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearProvisioningSecret(CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerIdFromClaims();
        await _adminSettingsService.ClearProvisioningSecretAsync(ownerId, cancellationToken);
        return Ok(new { message = "Provisioning secret cleared" });
    }

    /// <summary>
    /// Enables or disables provisioning for the calling owner.
    /// Requires a secret to be configured before enabling.
    /// </summary>
    [HttpPost("provisioning/enabled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetProvisioningEnabled(
        [FromBody] UpdateProvisioningEnabledRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetOwnerIdFromClaims();
            await _adminSettingsService.SetProvisioningEnabledAsync(ownerId, request.Enabled, cancellationToken);
            return Ok(new { message = request.Enabled ? "Provisioning enabled" : "Provisioning disabled" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion
}
