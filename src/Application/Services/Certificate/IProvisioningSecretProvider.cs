namespace Application.Services.Certificate;

/// <summary>
/// Result of a successful provisioning-secret validation.
/// Contains the owner resolved from the matched secret.
/// </summary>
public class ProvisioningSecretValidationResult
{
    public required Guid OwnerId { get; init; }
    public required string OwnerName { get; init; }
}

/// <summary>
/// Provider for provisioning secret validation.
/// Abstracts the storage of the secret (config file, database, etc.).
/// </summary>
public interface IProvisioningSecretProvider
{
    /// <summary>
    /// Validates a provisioning secret and returns the matching owner, or null if invalid.
    /// </summary>
    Task<ProvisioningSecretValidationResult?> ValidateSecretAsync(string secret, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if provisioning is currently enabled.
    /// </summary>
    Task<bool> IsProvisioningEnabledAsync(CancellationToken cancellationToken);
}
