#nullable enable
using Core.Entities.BaseEntities;

namespace Core.Entities.Settings;

/// <summary>
/// Stores system-wide settings that persist across restarts.
/// Uses a key-value pattern for flexibility.
/// </summary>
public class SystemSettings : Entity
{
    private SystemSettings() { }

    /// <summary>
    /// Setting key (unique identifier).
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Setting value (stored as string, parsed as needed).
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Optional description of the setting.
    /// </summary>
    public string? Description { get; set; }

    // Well-known setting keys
    public const string OtaFirmwareVersion = "Ota.FirmwareVersion";
    public const string OtaLastUpdatedUtc = "Ota.LastUpdatedUtc";
    public const string ProvisioningEnabled = "Provisioning.Enabled";
    public const string ProvisioningSecretHash = "Provisioning.SecretHash";
    public const string ProvisioningAutoCreateDevice = "Provisioning.AutoCreateDevice";

    public static SystemSettings Create(string key, string? value, string? description = null)
    {
        var setting = CreateEntity<SystemSettings>();
        setting.Key = key;
        setting.Value = value;
        setting.Description = description;
        return setting;
    }
}
