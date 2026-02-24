namespace Application.Services.Ota;

/// <summary>
/// Service for handling OTA (Over-The-Air) firmware updates.
/// </summary>
public interface IOtaService
{
    /// <summary>
    /// Gets the current firmware version string.
    /// </summary>
    /// <returns>Version string in format major.minor.patch</returns>
    string GetFirmwareVersion();

    /// <summary>
    /// Gets the firmware binary as a stream.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stream containing the firmware binary, or null if not found</returns>
    Task<Stream?> GetFirmwareStreamAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the firmware binary file path if it exists.
    /// </summary>
    /// <returns>File path if exists, null otherwise</returns>
    string? GetFirmwareFilePath();

    /// <summary>
    /// Validates if the firmware file exists and is valid.
    /// </summary>
    /// <returns>True if firmware is available and valid</returns>
    bool IsFirmwareAvailable();
}
