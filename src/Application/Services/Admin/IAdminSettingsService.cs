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

    // Provisioning Settings (per-owner)

    Task<ProvisioningSettingsResponse> GetProvisioningSettingsAsync(Guid ownerId, CancellationToken cancellationToken);

    Task UpdateProvisioningSecretAsync(Guid ownerId, string newSecret, string? label, CancellationToken cancellationToken);

    Task ClearProvisioningSecretAsync(Guid ownerId, CancellationToken cancellationToken);

    Task SetProvisioningEnabledAsync(Guid ownerId, bool enabled, CancellationToken cancellationToken);
}
