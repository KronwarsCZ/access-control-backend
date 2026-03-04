using ApiShared.Dto.Admin;

namespace Application.Services.Admin;

/// <summary>
/// Service for managing admin settings (OTA, provisioning, etc.).
/// </summary>
public interface IAdminSettingsService
{
    // OTA Settings (per-owner)

    Task<OtaSettingsResponse> GetOtaSettingsAsync(Guid ownerId, CancellationToken cancellationToken);

    Task UpdateFirmwareVersionAsync(Guid ownerId, string version, CancellationToken cancellationToken);

    Task SaveFirmwareAsync(Guid ownerId, Stream firmwareStream, string version, CancellationToken cancellationToken);

    Task SetOtaEnabledAsync(Guid ownerId, bool enabled, CancellationToken cancellationToken);

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
