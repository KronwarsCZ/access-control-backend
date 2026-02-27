using Application.Services.Admin;
using Application.Services.Ota;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccessControl.Web.Api.V1.IoT;

/// <summary>
/// Controller for OTA (Over-The-Air) firmware updates.
/// These endpoints are public (no auth) but must be accessed via HTTPS.
/// </summary>
[ApiController]
[Route("api/v1/ota")]
[AllowAnonymous]
public class OtaController : ControllerBase
{
    private readonly IOtaService _otaService;
    private readonly IAdminSettingsService _adminSettingsService;
    private readonly ILogger<OtaController> _logger;

    public OtaController(
        IOtaService otaService,
        IAdminSettingsService adminSettingsService,
        ILogger<OtaController> logger)
    {
        _otaService = otaService;
        _adminSettingsService = adminSettingsService;
        _logger = logger;
    }

    /// <summary>
    /// Returns the latest available firmware version.
    /// Device compares this with its current version and triggers update if server version is newer.
    /// </summary>
    /// <returns>Version string</returns>
    [HttpGet("version.txt")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFirmwareVersion(CancellationToken cancellationToken)
    {
        var settings = await _adminSettingsService.GetOtaSettingsAsync(cancellationToken);
        if (!settings.OtaEnabled)
        {
            _logger.LogDebug("OTA version check: returning 0.0.0 (OTA is disabled)");
            return Content("0.0.0", "text/plain");
        }

        var version = settings.FirmwareVersion;
        
        _logger.LogDebug("OTA version check: returning version {Version}", version);
        
        var safeVersion = version.Length > 31 ? version[..31] : version;
        
        return Content(safeVersion, "text/plain");
    }

    /// <summary>
    /// Downloads the firmware binary for OTA update.
    /// Returns the ESP32 application binary image.
    /// </summary>
    /// <returns>Firmware binary stream</returns>
    [HttpGet("firmware.bin")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "application/octet-stream")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetFirmwareBinary(CancellationToken cancellationToken)
    {
        var settings = await _adminSettingsService.GetOtaSettingsAsync(cancellationToken);
        if (!settings.OtaEnabled)
        {
            _logger.LogDebug("Firmware download rejected: OTA is disabled");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "OTA updates are disabled" });
        }

        if (!_otaService.IsFirmwareAvailable())
        {
            _logger.LogWarning("Firmware binary not available");
            return NotFound(new { error = "Firmware not available" });
        }

        try
        {
            var firmwarePath = _otaService.GetFirmwareFilePath();
            if (firmwarePath == null)
            {
                _logger.LogWarning("Firmware file path is null");
                return NotFound(new { error = "Firmware not found" });
            }

            var stream = await _otaService.GetFirmwareStreamAsync(cancellationToken);
            if (stream == null)
            {
                _logger.LogWarning("Could not open firmware stream");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { error = "Firmware temporarily unavailable" });
            }

            _logger.LogInformation("Serving firmware binary from {Path}", firmwarePath);
            
            // Return file stream for efficient transfer
            // EnableRangeProcessing allows resume of interrupted downloads
            return File(stream, "application/octet-stream", "firmware.bin", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serving firmware binary");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                new { error = "Error retrieving firmware" });
        }
    }

    /// <summary>
    /// Health check endpoint for OTA service.
    /// Returns whether firmware is available.
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var isAvailable = _otaService.IsFirmwareAvailable();
        var settings = await _adminSettingsService.GetOtaSettingsAsync(cancellationToken);
        var version = settings.FirmwareVersion;
        
        if (isAvailable)
        {
            return Ok(new 
            { 
                status = "healthy",
                firmwareAvailable = true,
                version = version
            });
        }
        
        return StatusCode(StatusCodes.Status503ServiceUnavailable, new 
        { 
            status = "degraded",
            firmwareAvailable = false,
            version = version
        });
    }
}
