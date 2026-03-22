namespace Application.Services.Ota;

/// <summary>
/// Service for handling OTA (Over-The-Air) firmware updates.
/// </summary>
public interface IOtaService
{
    /// <summary>
    /// Gets the current firmware version string (global/legacy fallback).
    /// </summary>
    string GetFirmwareVersion();

    /// <summary>
    /// Gets the firmware binary as a stream (global/legacy fallback).
    /// </summary>
    Task<Stream?> GetFirmwareStreamAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the firmware binary file path if it exists (global/legacy fallback).
    /// </summary>
    string? GetFirmwareFilePath();

    /// <summary>
    /// Validates if the firmware file exists and is valid (global/legacy fallback).
    /// </summary>
    bool IsFirmwareAvailable();

    /// <summary>
    /// Gets the per-owner firmware file path: firmware/{ownerId}/firmware.bin
    /// </summary>
    string? GetFirmwareFilePath(Guid ownerId);

    /// <summary>
    /// Gets the per-owner firmware binary as a stream.
    /// </summary>
    Task<Stream?> GetFirmwareStreamAsync(Guid ownerId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether firmware is available for a specific owner.
    /// </summary>
    bool IsFirmwareAvailable(Guid ownerId);
}
