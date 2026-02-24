namespace Application.Services.Ota;

/// <summary>
/// Configuration for OTA (Over-The-Air) firmware updates.
/// </summary>
public class OtaConfiguration
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Ota";

    /// <summary>
    /// Current firmware version string.
    /// Format: major.minor.patch (semantic versioning).
    /// Max length: 31 characters.
    /// </summary>
    public string FirmwareVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Path to the firmware binary file.
    /// Can be absolute or relative to the application directory.
    /// </summary>
    public string FirmwarePath { get; set; } = "firmware/firmware.bin";

    /// <summary>
    /// Maximum allowed firmware file size in bytes.
    /// Default: 4MB (typical ESP32 OTA partition size).
    /// </summary>
    public long MaxFirmwareSizeBytes { get; set; } = 4 * 1024 * 1024;
}
