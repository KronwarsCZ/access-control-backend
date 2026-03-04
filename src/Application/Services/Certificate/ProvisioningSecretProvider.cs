using System.Security.Cryptography;
using System.Text;
using Application.IUnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Certificate;

/// <summary>
/// Validates provisioning secrets against per-owner records in the database,
/// with a config-file fallback for development.
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
    public async Task<ProvisioningSecretValidationResult?> ValidateSecretAsync(
        string secret, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(secret))
            return null;

        if (!await IsProvisioningEnabledAsync(cancellationToken))
        {
            _logger.LogWarning("Provisioning is disabled");
            return null;
        }

        // Hash the incoming secret and look it up among per-owner secrets
        var providedHash = HashSecret(secret);

        var allActive = await _unitOfWork.ProvisioningSecretRepository
            .GetAllActiveAsync(cancellationToken);

        foreach (var entry in allActive)
        {
            var storedBytes = Convert.FromBase64String(entry.SecretHash);
            var providedBytes = Convert.FromBase64String(providedHash);

            if (CryptographicOperations.FixedTimeEquals(storedBytes, providedBytes))
            {
                return new ProvisioningSecretValidationResult
                {
                    OwnerId = entry.OwnerId,
                    OwnerName = entry.Owner.Name
                };
            }
        }

        _logger.LogWarning("No matching provisioning secret found");
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> IsProvisioningEnabledAsync(CancellationToken cancellationToken)
    {
        var activeSecrets = await _unitOfWork.ProvisioningSecretRepository
            .GetAllActiveAsync(cancellationToken);

        if (activeSecrets.Any())
            return true;

        return !string.IsNullOrEmpty(_configuration.ProvisioningSecret);
    }

    /// <summary>
    /// Resolves the owner to use when the config-file provisioning secret matches.
    /// Uses DefaultOwnerId from config, otherwise falls back to the first owner.
    /// </summary>
    private async Task<Core.Entities.Owners.Owner?> ResolveConfigFallbackOwnerAsync(
        CancellationToken cancellationToken)
    {
        if (_configuration.DefaultOwnerId is { } id && id != Guid.Empty)
        {
            var owner = await _unitOfWork.OwnerRepository.GetByIdAsync(id, cancellationToken);
            if (owner != null) return owner;
            _logger.LogWarning("Configured DefaultOwnerId {OwnerId} not found", id);
        }

        var owners = await _unitOfWork.OwnerRepository
            .FindByConditionAsync(o => !o.IsDeleted, cancellationToken);
        return owners.FirstOrDefault();
    }

    internal static string HashSecret(string secret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(bytes);
    }
}
