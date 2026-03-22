namespace ApiShared.Dto.Admin;

/// <summary>
/// Response containing current OTA settings for a specific owner.
/// </summary>
public class OtaSettingsResponse
{
    public Guid OwnerId { get; init; }

    public string? OwnerName { get; init; }

    public string? FirmwareVersion { get; init; }

    public bool FirmwareAvailable { get; init; }

    public long? FirmwareSizeBytes { get; init; }

    public DateTime? LastUpdatedUtc { get; init; }

    public bool OtaEnabled { get; init; }
}
