namespace ApiShared.Dto.Admin;

/// <summary>
/// Response containing current provisioning settings.
/// </summary>
public class ProvisioningSettingsResponse
{
    public bool ProvisioningEnabled { get; init; }

    /// <summary>
    /// Whether the calling owner has a provisioning secret configured.
    /// </summary>
    public bool SecretConfigured { get; init; }

    public bool AutoCreateDevice { get; init; }

    public int ProvisionedDeviceCount { get; init; }

    /// <summary>
    /// The calling owner's provisioning secret info (if any).
    /// </summary>
    public ProvisioningSecretResponse? OwnerSecret { get; init; }
}
