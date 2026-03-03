using System.Security.Cryptography;
using System.Text;
using ApiShared.Dto.Admin;
using Application.IUnitOfWork;
using Application.Services.Ota;
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

    #region OTA Settings

    /// <inheritdoc />
    public async Task<OtaSettingsResponse> GetOtaSettingsAsync(CancellationToken cancellationToken)
    {
        var versionSetting = await GetSettingAsync(SystemSettings.OtaFirmwareVersion, cancellationToken);
        var lastUpdatedSetting = await GetSettingAsync(SystemSettings.OtaLastUpdatedUtc, cancellationToken);
        var enabledSetting = await GetSettingAsync(SystemSettings.OtaEnabled, cancellationToken);

        var hasDbVersion = !string.IsNullOrEmpty(versionSetting?.Value);
        var version = hasDbVersion ? versionSetting!.Value : _otaConfiguration.FirmwareVersion;

        // #region agent log
        try { var _logPayload = System.Text.Json.JsonSerializer.Serialize(new { sessionId = "227557", id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_h1", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AdminSettingsService.cs:GetOtaSettingsAsync", message = "DB settings query results", data = new { versionSettingIsNull = versionSetting == null, versionSettingValue = versionSetting?.Value, configFirmwareVersion = _otaConfiguration.FirmwareVersion, hasDbVersion, enabledSettingValue = enabledSetting?.Value }, runId = "post-fix", hypothesisId = "H1" }); System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "debug-227557.log"), _logPayload + "\n"); } catch { }
        // #endregion
        
        DateTime? lastUpdated = null;
        if (lastUpdatedSetting?.Value != null && DateTime.TryParse(lastUpdatedSetting.Value, out var parsed))
        {
            lastUpdated = parsed;
        }

        var firmwarePath = GetFirmwarePath();
        var firmwareFileExists = File.Exists(firmwarePath);
        var firmwareAvailable = hasDbVersion && firmwareFileExists;
        long? firmwareSize = null;
        
        if (firmwareFileExists)
        {
            var fileInfo = new FileInfo(firmwarePath);
            firmwareSize = fileInfo.Length;
        }

        // #region agent log
        try { var _logPayload2 = System.Text.Json.JsonSerializer.Serialize(new { sessionId = "227557", id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_h2", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AdminSettingsService.cs:GetOtaSettingsAsync", message = "OTA settings response values", data = new { resolvedVersion = version, firmwarePath, firmwareFileExists, firmwareAvailable, firmwareSize, otaEnabled = enabledSetting?.Value == "true", versionSource = hasDbVersion ? "database" : "config_fallback" }, runId = "post-fix", hypothesisId = "H2" }); System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "debug-227557.log"), _logPayload2 + "\n"); } catch { }
        // #endregion

        return new OtaSettingsResponse
        {
            FirmwareVersion = hasDbVersion ? version : null,
            FirmwareAvailable = firmwareAvailable,
            FirmwareSizeBytes = firmwareAvailable ? firmwareSize : null,
            LastUpdatedUtc = lastUpdated,
            OtaEnabled = enabledSetting?.Value == "true"
        };
    }

    /// <inheritdoc />
    public async Task UpdateFirmwareVersionAsync(string version, CancellationToken cancellationToken)
    {
        await SetSettingAsync(SystemSettings.OtaFirmwareVersion, version, "Firmware version", cancellationToken);
        await SetSettingAsync(SystemSettings.OtaLastUpdatedUtc, DateTime.UtcNow.ToString("O"), "Last firmware update", cancellationToken);
        
        _logger.LogInformation("Firmware version updated to {Version}", version);
    }

    /// <inheritdoc />
    public async Task SaveFirmwareAsync(Stream firmwareStream, string version, CancellationToken cancellationToken)
    {
        var firmwarePath = GetFirmwarePath();
        var firmwareDir = Path.GetDirectoryName(firmwarePath);
        
        if (!string.IsNullOrEmpty(firmwareDir) && !Directory.Exists(firmwareDir))
        {
            Directory.CreateDirectory(firmwareDir);
        }

        // Validate file size
        if (firmwareStream.Length > _otaConfiguration.MaxFirmwareSizeBytes)
        {
            throw new InvalidOperationException($"Firmware file exceeds maximum size of {_otaConfiguration.MaxFirmwareSizeBytes} bytes");
        }

        // Save with a temporary name first, then rename (atomic operation)
        var tempPath = firmwarePath + ".tmp";
        
        await using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await firmwareStream.CopyToAsync(fileStream, cancellationToken);
        }

        // Replace the old file
        if (File.Exists(firmwarePath))
        {
            File.Delete(firmwarePath);
        }
        File.Move(tempPath, firmwarePath);

        // Update version
        await UpdateFirmwareVersionAsync(version, cancellationToken);

        _logger.LogInformation("Firmware saved: {Path}, Version: {Version}, Size: {Size} bytes", 
            firmwarePath, version, firmwareStream.Length);
    }

    /// <inheritdoc />
    public async Task SetOtaEnabledAsync(bool enabled, CancellationToken cancellationToken)
    {
        if (enabled)
        {
            var versionSetting = await GetSettingAsync(SystemSettings.OtaFirmwareVersion, cancellationToken);
            var firmwarePath = GetFirmwarePath();
            var firmwareExists = File.Exists(firmwarePath);

            if (string.IsNullOrEmpty(versionSetting?.Value) || !firmwareExists)
            {
                throw new InvalidOperationException("Cannot enable OTA without a firmware version set and firmware file uploaded");
            }
        }

        await SetSettingAsync(SystemSettings.OtaEnabled, enabled ? "true" : "false", "OTA enabled", cancellationToken);
        
        _logger.LogInformation("OTA {Status}", enabled ? "enabled" : "disabled");
    }

    private string GetFirmwarePath()
    {
        var configuredPath = _otaConfiguration.FirmwarePath;
        return Path.IsPathRooted(configuredPath) 
            ? configuredPath 
            : Path.Combine(_basePath, configuredPath);
    }

    #endregion

    #region Provisioning Settings

    /// <inheritdoc />
    public async Task<ProvisioningSettingsResponse> GetProvisioningSettingsAsync(CancellationToken cancellationToken)
    {
        var enabledSetting = await GetSettingAsync(SystemSettings.ProvisioningEnabled, cancellationToken);
        var secretSetting = await GetSettingAsync(SystemSettings.ProvisioningSecretHash, cancellationToken);
        var autoCreateSetting = await GetSettingAsync(SystemSettings.ProvisioningAutoCreateDevice, cancellationToken);

        var activeDevices = await _unitOfWork.ProvisionedDeviceRepository.GetAllActiveAsync(cancellationToken);
        var provisionedCount = activeDevices.Count();

        return new ProvisioningSettingsResponse
        {
            ProvisioningEnabled = enabledSetting?.Value == "true",
            SecretConfigured = !string.IsNullOrEmpty(secretSetting?.Value),
            AutoCreateDevice = autoCreateSetting?.Value != "false", // default true
            ProvisionedDeviceCount = provisionedCount
        };
    }

    /// <inheritdoc />
    public async Task UpdateProvisioningSecretAsync(string newSecret, CancellationToken cancellationToken)
    {
        // Hash the secret before storing (we never store plaintext)
        var hashedSecret = HashSecret(newSecret);
        
        await SetSettingAsync(SystemSettings.ProvisioningSecretHash, hashedSecret, "Provisioning secret hash", cancellationToken);
        await SetSettingAsync(SystemSettings.ProvisioningEnabled, "true", "Provisioning enabled", cancellationToken);
        
        _logger.LogInformation("Provisioning secret updated and provisioning enabled");
    }

    /// <inheritdoc />
    public async Task ClearProvisioningSecretAsync(CancellationToken cancellationToken)
    {
        await SetSettingAsync(SystemSettings.ProvisioningSecretHash, null, "Provisioning secret hash", cancellationToken);
        await SetSettingAsync(SystemSettings.ProvisioningEnabled, "false", "Provisioning enabled", cancellationToken);
        
        _logger.LogInformation("Provisioning secret cleared and provisioning disabled");
    }

    /// <inheritdoc />
    public async Task SetProvisioningEnabledAsync(bool enabled, CancellationToken cancellationToken)
    {
        // Can only enable if secret is configured
        if (enabled)
        {
            var secretSetting = await GetSettingAsync(SystemSettings.ProvisioningSecretHash, cancellationToken);
            if (string.IsNullOrEmpty(secretSetting?.Value))
            {
                throw new InvalidOperationException("Cannot enable provisioning without a secret configured");
            }
        }

        await SetSettingAsync(SystemSettings.ProvisioningEnabled, enabled ? "true" : "false", "Provisioning enabled", cancellationToken);
        
        _logger.LogInformation("Provisioning {Status}", enabled ? "enabled" : "disabled");
    }

    private static string HashSecret(string secret)
    {
        // Use SHA256 for hashing the secret
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(bytes);
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
