using Application.IRepositories.Devices;
using Application.IRepositories.Ota;
using Application.Services.Ota;
using Application.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccessControl.Web.Api.V1.IoT;

/// <summary>
/// Controller for OTA (Over-The-Air) firmware updates.
/// Devices identify themselves via X-Device-ID header; firmware is served per-owner.
/// </summary>
[ApiController]
[Route("api/v1/ota")]
[AllowAnonymous]
public class OtaController : ControllerBase
{
    private readonly IOtaService _otaService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OtaController> _logger;

    public OtaController(
        IOtaService otaService,
        IUnitOfWork unitOfWork,
        ILogger<OtaController> logger)
    {
        _otaService = otaService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("version.txt")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFirmwareVersion(CancellationToken cancellationToken)
    {
        var ownerId = await ResolveOwnerIdFromDeviceHeaderAsync(cancellationToken);
        if (ownerId == null)
        {
            _logger.LogDebug("OTA version check: returning 0.0.0 (device not identified)");
            return Content("0.0.0", "text/plain");
        }

        var ownerFirmware = await _unitOfWork.OwnerFirmwareRepository
            .GetByOwnerIdAsync(ownerId.Value, cancellationToken);

        if (ownerFirmware == null || !ownerFirmware.IsEnabled)
        {
            _logger.LogDebug("OTA version check: returning 0.0.0 (disabled or not configured for owner {OwnerId})", ownerId);
            return Content("0.0.0", "text/plain");
        }

        var version = ownerFirmware.FirmwareVersion;
        if (string.IsNullOrEmpty(version))
        {
            _logger.LogDebug("OTA version check: returning 0.0.0 (no version for owner {OwnerId})", ownerId);
            return Content("0.0.0", "text/plain");
        }

        _logger.LogDebug("OTA version check: returning {Version} for owner {OwnerId}", version, ownerId);
        var safeVersion = version.Length > 31 ? version[..31] : version;
        return Content(safeVersion, "text/plain");
    }

    [HttpGet("firmware.bin")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "application/octet-stream")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetFirmwareBinary(CancellationToken cancellationToken)
    {
        var ownerId = await ResolveOwnerIdFromDeviceHeaderAsync(cancellationToken);
        if (ownerId == null)
            return NotFound(new { error = "Device not identified" });

        var ownerFirmware = await _unitOfWork.OwnerFirmwareRepository
            .GetByOwnerIdAsync(ownerId.Value, cancellationToken);

        if (ownerFirmware == null || !ownerFirmware.IsEnabled)
        {
            _logger.LogDebug("Firmware download rejected: OTA disabled for owner {OwnerId}", ownerId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "OTA updates are disabled" });
        }

        if (!_otaService.IsFirmwareAvailable(ownerId.Value))
        {
            _logger.LogWarning("Firmware binary not available for owner {OwnerId}", ownerId);
            return NotFound(new { error = "Firmware not available" });
        }

        try
        {
            var stream = await _otaService.GetFirmwareStreamAsync(ownerId.Value, cancellationToken);
            if (stream == null)
            {
                _logger.LogWarning("Could not open firmware stream for owner {OwnerId}", ownerId);
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "Firmware temporarily unavailable" });
            }

            _logger.LogInformation("Serving firmware binary for owner {OwnerId}", ownerId);
            return File(stream, "application/octet-stream", "firmware.bin", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serving firmware binary for owner {OwnerId}", ownerId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "Error retrieving firmware" });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var ownerId = await ResolveOwnerIdFromDeviceHeaderAsync(cancellationToken);

        if (ownerId == null)
        {
            return Ok(new { status = "unknown", reason = "device not identified" });
        }

        var ownerFirmware = await _unitOfWork.OwnerFirmwareRepository
            .GetByOwnerIdAsync(ownerId.Value, cancellationToken);
        var isAvailable = _otaService.IsFirmwareAvailable(ownerId.Value);

        if (ownerFirmware is { IsEnabled: true } && isAvailable)
        {
            return Ok(new
            {
                status = "healthy",
                firmwareAvailable = true,
                version = ownerFirmware.FirmwareVersion
            });
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new
        {
            status = "degraded",
            firmwareAvailable = isAvailable,
            version = ownerFirmware?.FirmwareVersion
        });
    }

    private async Task<Guid?> ResolveOwnerIdFromDeviceHeaderAsync(CancellationToken cancellationToken)
    {
        var deviceId = Request.Headers["X-Device-ID"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            _logger.LogDebug("No X-Device-ID header present");
            return null;
        }

        var device = await _unitOfWork.DeviceRepository.GetByHardwareIdAsync(deviceId, cancellationToken);
        if (device == null)
        {
            _logger.LogWarning("Device not found for hardware ID: {DeviceId}", deviceId);
            return null;
        }

        return device.OwnerId;
    }
}
