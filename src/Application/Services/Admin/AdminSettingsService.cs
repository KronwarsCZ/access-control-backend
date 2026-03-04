using ApiShared.Dto.Admin;
using Application.IUnitOfWork;
using Application.Services.Certificate;
using Application.Services.Ota;
using Core.Entities.DeviceProvisioning;
using Core.Entities.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Admin;

/// <summary>
/// Implementation of admin settings service.
/// </summary>
public class AdminSettingsService : IAdminSettingsService
{
    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    private readonly OtaConfiguration _otaConfiguration;
    private readonly ILogger<AdminSettingsService> _logger;
    private readonly string _basePath;

    public AdminSettingsService(
        IUnitOfWork.IUnitOfWork unitOfWork,
        IOptions<OtaConfiguration> otaConfiguration,
        ILogger<AdminSettingsService> logger)
    {
        _unitOfWork = unitOfWork;
        _otaConfiguration = otaConfiguration.Value;
        _logger = logger;
        _basePath = AppDomain.CurrentDomain.BaseDirectory;
    }

    #region OTA Settings (per-owner)

    /// <inheritdoc />
    public async Task<OtaSettingsResponse> GetOtaSettingsAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var ownerFirmware = await _unitOfWork.OwnerFirmwareRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        var firmwarePath = GetFirmwarePath(ownerId);
        var firmwareFileExists = File.Exists(firmwarePath);
        long? firmwareSize = firmwareFileExists ? new FileInfo(firmwarePath).Length : null;

        var hasVersion = ownerFirmware != null && !string.IsNullOrEmpty(ownerFirmware.FirmwareVersion);
        var firmwareAvailable = hasVersion && firmwareFileExists;

        return new OtaSettingsResponse
        {
            OwnerId = ownerId,
            OwnerName = ownerFirmware?.Owner?.Name,
            FirmwareVersion = hasVersion ? ownerFirmware!.FirmwareVersion : null,
            FirmwareAvailable = firmwareAvailable,
            FirmwareSizeBytes = firmwareAvailable ? firmwareSize : null,
            LastUpdatedUtc = ownerFirmware?.LastUpdatedUtc,
            OtaEnabled = ownerFirmware?.IsEnabled ?? false
        };
    }

    /// <inheritdoc />
    public async Task UpdateFirmwareVersionAsync(Guid ownerId, string version, CancellationToken cancellationToken)
    {
        var ownerFirmware = await GetOrCreateOwnerFirmwareAsync(ownerId, cancellationToken);
        ownerFirmware.FirmwareVersion = version;
        ownerFirmware.LastUpdatedUtc = DateTime.UtcNow;

        _unitOfWork.OwnerFirmwareRepository.Update([ownerFirmware]);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Firmware version updated to {Version} for owner {OwnerId}", version, ownerId);
    }

    /// <inheritdoc />
    public async Task SaveFirmwareAsync(Guid ownerId, Stream firmwareStream, string version, CancellationToken cancellationToken)
    {
        if (firmwareStream.Length > _otaConfiguration.MaxFirmwareSizeBytes)
        {
            throw new InvalidOperationException(
                $"Firmware file exceeds maximum size of {_otaConfiguration.MaxFirmwareSizeBytes} bytes");
        }

        var firmwarePath = GetFirmwarePath(ownerId);
        var firmwareDir = Path.GetDirectoryName(firmwarePath)!;

        if (!Directory.Exists(firmwareDir))
            Directory.CreateDirectory(firmwareDir);

        var tempPath = firmwarePath + ".tmp";

        await using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await firmwareStream.CopyToAsync(fileStream, cancellationToken);
        }

        if (File.Exists(firmwarePath))
            File.Delete(firmwarePath);
        File.Move(tempPath, firmwarePath);

        var ownerFirmware = await GetOrCreateOwnerFirmwareAsync(ownerId, cancellationToken);
        ownerFirmware.FirmwareVersion = version;
        ownerFirmware.IsEnabled = true;
        ownerFirmware.LastUpdatedUtc = DateTime.UtcNow;

        _unitOfWork.OwnerFirmwareRepository.Update([ownerFirmware]);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Firmware saved and OTA enabled for owner {OwnerId}: {Path}, Version: {Version}, Size: {Size} bytes",
            ownerId, firmwarePath, version, firmwareStream.Length);
    }

    /// <inheritdoc />
    public async Task SetOtaEnabledAsync(Guid ownerId, bool enabled, CancellationToken cancellationToken)
    {
        var ownerFirmware = await _unitOfWork.OwnerFirmwareRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (enabled)
        {
            if (ownerFirmware == null || string.IsNullOrEmpty(ownerFirmware.FirmwareVersion))
                throw new InvalidOperationException("Cannot enable OTA without a firmware version set");

            var firmwarePath = GetFirmwarePath(ownerId);
            if (!File.Exists(firmwarePath))
                throw new InvalidOperationException("Cannot enable OTA without a firmware file uploaded");
        }

        if (ownerFirmware == null)
            throw new InvalidOperationException("No firmware record exists for this owner");

        ownerFirmware.IsEnabled = enabled;
        _unitOfWork.OwnerFirmwareRepository.Update([ownerFirmware]);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OTA {Status} for owner {OwnerId}", enabled ? "enabled" : "disabled", ownerId);
    }

    private string GetFirmwarePath(Guid ownerId)
    {
        return Path.Combine(_basePath, "firmware", ownerId.ToString(), "firmware.bin");
    }

    private async Task<Core.Entities.Ota.OwnerFirmware> GetOrCreateOwnerFirmwareAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.OwnerFirmwareRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (existing != null)
            return existing;

        var created = Core.Entities.Ota.OwnerFirmware.Create(ownerId, "0.0.0");
        _unitOfWork.OwnerFirmwareRepository.Add([created]);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return created;
    }

    #endregion

    #region Provisioning Settings (per-owner)

    /// <inheritdoc />
    public async Task<ProvisioningSettingsResponse> GetProvisioningSettingsAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var autoCreateSetting = await GetSettingAsync(SystemSettings.ProvisioningAutoCreateDevice, cancellationToken);

        var activeDevices = await _unitOfWork.ProvisionedDeviceRepository.GetAllActiveAsync(cancellationToken);
        var provisionedCount = activeDevices.Count();

        var ownerSecret = await _unitOfWork.ProvisioningSecretRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        return new ProvisioningSettingsResponse
        {
            ProvisioningEnabled = ownerSecret is { IsActive: true },
            SecretConfigured = ownerSecret != null && !ownerSecret.IsDeleted,
            AutoCreateDevice = autoCreateSetting?.Value != "false",
            ProvisionedDeviceCount = provisionedCount,
            OwnerSecret = ownerSecret != null
                ? new ProvisioningSecretResponse
                {
                    Id = ownerSecret.Id,
                    OwnerId = ownerSecret.OwnerId,
                    OwnerName = ownerSecret.Owner?.Name,
                    Label = ownerSecret.Label,
                    IsActive = ownerSecret.IsActive,
                    CreatedUtc = ownerSecret.CreatedUtc
                }
                : null
        };
    }

    /// <inheritdoc />
    public async Task UpdateProvisioningSecretAsync(Guid ownerId, string newSecret, string? label, CancellationToken cancellationToken)
    {
        var hashedSecret = ProvisioningSecretProvider.HashSecret(newSecret);

        // Reject if another owner already uses this exact secret
        var duplicate = await _unitOfWork.ProvisioningSecretRepository.GetBySecretHashAsync(hashedSecret, cancellationToken);
        if (duplicate != null && duplicate.OwnerId != ownerId)
            throw new InvalidOperationException("This secret is already in use by another owner. Please choose a different secret.");

        var existing = await _unitOfWork.ProvisioningSecretRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (existing != null)
        {
            existing.SecretHash = hashedSecret;
            existing.Label = label ?? existing.Label;
            existing.IsActive = true;
            _unitOfWork.ProvisioningSecretRepository.Update([existing]);
        }
        else
        {
            var secret = ProvisioningSecret.Create(hashedSecret, ownerId, label);
            _unitOfWork.ProvisioningSecretRepository.Add([secret]);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Provisioning secret set and enabled for owner {OwnerId}", ownerId);
    }

    /// <inheritdoc />
    public async Task ClearProvisioningSecretAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.ProvisioningSecretRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (existing != null)
        {
            _unitOfWork.ProvisioningSecretRepository.Delete([existing]);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Provisioning secret cleared for owner {OwnerId}", ownerId);
    }

    /// <inheritdoc />
    public async Task SetProvisioningEnabledAsync(Guid ownerId, bool enabled, CancellationToken cancellationToken)
    {
        var ownerSecret = await _unitOfWork.ProvisioningSecretRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (enabled && ownerSecret == null)
            throw new InvalidOperationException("Cannot enable provisioning without a secret configured");

        if (ownerSecret != null)
        {
            ownerSecret.IsActive = enabled;
            _unitOfWork.ProvisioningSecretRepository.Update([ownerSecret]);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Provisioning {Status} for owner {OwnerId}", enabled ? "enabled" : "disabled", ownerId);
    }

    #endregion

    #region Helper Methods

    private async Task<SystemSettings?> GetSettingAsync(string key, CancellationToken cancellationToken)
    {
        return await _unitOfWork.SystemSettingsRepository.GetByKeyAsync(key, cancellationToken);
    }

    private async Task SetSettingAsync(string key, string? value, string? description, CancellationToken cancellationToken)
    {
        await _unitOfWork.SystemSettingsRepository.SetAsync(key, value, description, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
