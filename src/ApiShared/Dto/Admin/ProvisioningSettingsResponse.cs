namespace ApiShared.Dto.Admin;

/// <summary>
/// Response containing current provisioning settings.
/// Note: The actual secret is never returned for security.
/// </summary>
public class ProvisioningSettingsResponse
{
    /// <summary>
    /// Whether provisioning is currently enabled.
    /// </summary>
    public bool ProvisioningEnabled { get; init; }

    /// <summary>
    /// Whether a provisioning secret is configured.
    /// </summary>
    public bool SecretConfigured { get; init; }

    /// <summary>
    /// Whether auto-creation of devices is enabled.
    /// </summary>
    public bool AutoCreateDevice { get; init; }

    /// <summary>
    /// Number of devices provisioned.
    /// </summary>
    public int ProvisionedDeviceCount { get; init; }
}
