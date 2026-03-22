#nullable enable
using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Core.Entities.Ota;

/// <summary>
/// Per-owner firmware settings for OTA updates.
/// Each owner can have a different firmware version and binary.
/// </summary>
public class OwnerFirmware : Entity
{
    private OwnerFirmware() { }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    /// <summary>
    /// Firmware version string (semantic versioning, e.g. "2.1.0").
    /// </summary>
    public string FirmwareVersion { get; set; } = string.Empty;

    /// <summary>
    /// Whether OTA updates are enabled for this owner's devices.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// When the firmware was last updated for this owner.
    /// </summary>
    public DateTime? LastUpdatedUtc { get; set; }

    public static OwnerFirmware Create(Guid ownerId, string firmwareVersion)
    {
        var entity = CreateEntity<OwnerFirmware>();
        entity.OwnerId = ownerId;
        entity.FirmwareVersion = firmwareVersion;
        entity.IsEnabled = false;
        entity.LastUpdatedUtc = DateTime.UtcNow;
        return entity;
    }
}
