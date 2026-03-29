namespace Application.Services.Certificate;

/// <summary>
/// Service for certificate signing and validation.
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Signs a CSR and returns a client certificate.
    /// </summary>
    /// <param name="csrPem">PEM-encoded CSR</param>
    /// <param name="deviceId">Device identifier (expected CN)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PEM-encoded signed certificate</returns>
    Task<string> SignCsrAsync(string csrPem, string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the CA certificate in PEM format.
    /// </summary>
    /// <returns>PEM-encoded CA certificate</returns>
    string GetCaCertificate();

    /// <summary>
    /// Validates a CSR format and content.
    /// </summary>
    /// <param name="csrPem">PEM-encoded CSR</param>
    /// <param name="expectedCn">Expected Common Name</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateCsr(string csrPem, string expectedCn);

    /// <summary>
    /// Checks if the certificate service is properly configured.
    /// </summary>
    /// <returns>True if CA key and certificate are available</returns>
    bool IsConfigured();
}
