using System.Security.Cryptography;
using System.Text;
using Application.IUnitOfWork;
using Microsoft.Extensions.Logging;
namespace Application.Services.Certificate;

/// <summary>
/// Validates provisioning secrets against per-owner records in the database.
/// </summary>
public class ProvisioningSecretProvider : IProvisioningSecretProvider
{
    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    private readonly ILogger<ProvisioningSecretProvider> _logger;

    public ProvisioningSecretProvider(
        IUnitOfWork.IUnitOfWork unitOfWork,
        ILogger<ProvisioningSecretProvider> logger)
    {
        _unitOfWork = unitOfWork;
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

        return activeSecrets.Any();
    }

    internal static string HashSecret(string secret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(bytes);
    }
}
