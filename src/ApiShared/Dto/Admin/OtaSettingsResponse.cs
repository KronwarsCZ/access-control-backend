namespace ApiShared.Dto.Admin;

/// <summary>
/// Response containing current OTA settings.
/// </summary>
public class OtaSettingsResponse
{
    /// <summary>
    /// Current firmware version string.
    /// </summary>
    public string FirmwareVersion { get; init; } = string.Empty;

    /// <summary>
    /// Whether firmware file is available.
    /// </summary>
    public bool FirmwareAvailable { get; init; }

    /// <summary>
    /// Firmware file size in bytes (if available).
    /// </summary>
    public long? FirmwareSizeBytes { get; init; }

    /// <summary>
    /// Last time firmware was updated.
    /// </summary>
    public DateTime? LastUpdatedUtc { get; init; }

    /// <summary>
    /// Whether OTA updates are enabled for devices.
    /// </summary>
    public bool OtaEnabled { get; init; }
}
