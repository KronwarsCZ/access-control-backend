namespace Application.Services.Certificate;

/// <summary>
/// Provider for provisioning secret validation.
/// Abstracts the storage of the secret (config file, database, etc.).
/// </summary>
public interface IProvisioningSecretProvider
{
    /// <summary>
    /// Validates a provisioning secret against the stored secret.
    /// </summary>
    /// <param name="secret">The secret to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid</returns>
    Task<bool> ValidateSecretAsync(string secret, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if provisioning is currently enabled.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if enabled</returns>
    Task<bool> IsProvisioningEnabledAsync(CancellationToken cancellationToken);
}
