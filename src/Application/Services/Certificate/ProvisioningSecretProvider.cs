using System.Security.Cryptography;
using System.Text;
using Application.IUnitOfWork;
using Core.Entities.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Certificate;

/// <summary>
/// Provides provisioning secret validation from database or config.
/// Database settings take precedence over config file.
/// </summary>
public class ProvisioningSecretProvider : IProvisioningSecretProvider
{
    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    private readonly CertificateConfiguration _configuration;
    private readonly ILogger<ProvisioningSecretProvider> _logger;

    public ProvisioningSecretProvider(
        IUnitOfWork.IUnitOfWork unitOfWork,
        IOptions<CertificateConfiguration> configuration,
        ILogger<ProvisioningSecretProvider> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateSecretAsync(string secret, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(secret))
        {
            return false;
        }

        // First check if provisioning is enabled
        if (!await IsProvisioningEnabledAsync(cancellationToken))
        {
            _logger.LogWarning("Provisioning is disabled");
            return false;
        }

        // Try database first
        var storedHashSetting = await _unitOfWork.SystemSettingsRepository
            .GetByKeyAsync(SystemSettings.ProvisioningSecretHash, cancellationToken);

        if (!string.IsNullOrEmpty(storedHashSetting?.Value))
        {
            // Compare with stored hash
            var providedHash = HashSecret(secret);
            
            // Use constant-time comparison
            var storedBytes = Convert.FromBase64String(storedHashSetting.Value);
            var providedBytes = Convert.FromBase64String(providedHash);
            
            return CryptographicOperations.FixedTimeEquals(storedBytes, providedBytes);
        }

        // Fall back to config file (for backward compatibility / development)
        if (!string.IsNullOrEmpty(_configuration.ProvisioningSecret))
        {
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(secret),
                Encoding.UTF8.GetBytes(_configuration.ProvisioningSecret));
        }

        _logger.LogWarning("No provisioning secret configured");
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> IsProvisioningEnabledAsync(CancellationToken cancellationToken)
    {
        // Check database setting first
        var enabledSetting = await _unitOfWork.SystemSettingsRepository
            .GetByKeyAsync(SystemSettings.ProvisioningEnabled, cancellationToken);

        if (enabledSetting != null)
        {
            return enabledSetting.Value == "true";
        }

        // If no database setting, check if config has a secret (implicit enabled)
        return !string.IsNullOrEmpty(_configuration.ProvisioningSecret);
    }

    private static string HashSecret(string secret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(bytes);
    }
}
