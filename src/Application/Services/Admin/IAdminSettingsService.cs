using ApiShared.Dto.Admin;

namespace Application.Services.Admin;

/// <summary>
/// Service for managing admin settings (OTA, provisioning, etc.).
/// </summary>
public interface IAdminSettingsService
{
    // OTA Settings
    
    /// <summary>
    /// Gets current OTA settings.
    /// </summary>
    Task<OtaSettingsResponse> GetOtaSettingsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Updates the firmware version.
    /// </summary>
    Task UpdateFirmwareVersionAsync(string version, CancellationToken cancellationToken);

    /// <summary>
    /// Saves an uploaded firmware file.
    /// </summary>
    Task SaveFirmwareAsync(Stream firmwareStream, string version, CancellationToken cancellationToken);

    /// <summary>
    /// Enables or disables OTA updates.
    /// </summary>
    Task SetOtaEnabledAsync(bool enabled, CancellationToken cancellationToken);

    // Provisioning Settings

    /// <summary>
    /// Gets current provisioning settings.
    /// </summary>
    Task<ProvisioningSettingsResponse> GetProvisioningSettingsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Updates the provisioning secret.
    /// </summary>
    Task UpdateProvisioningSecretAsync(string newSecret, CancellationToken cancellationToken);

    /// <summary>
    /// Clears the provisioning secret (disables provisioning).
    /// </summary>
    Task ClearProvisioningSecretAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Enables or disables provisioning.
    /// </summary>
    Task SetProvisioningEnabledAsync(bool enabled, CancellationToken cancellationToken);
}
