namespace Application.Services.Certificate;

/// <summary>
/// Configuration for certificate management and signing.
/// All certificate material (PEM) is loaded from environment variables at startup.
/// </summary>
public class CertificateConfiguration
{
    public const string SectionName = "Certificate";

    /// <summary>
    /// Password for the CA private key if encrypted.
    /// Leave empty for unencrypted keys.
    /// </summary>
    public string? CaPrivateKeyPassword { get; set; }

    /// <summary>
    /// Certificate validity period in days.
    /// Default: 365 days (1 year).
    /// </summary>
    public int CertificateValidityDays { get; set; } = 365;

    /// <summary>
    /// Whether to automatically create a Device entity when provisioning.
    /// Default: true.
    /// </summary>
    public bool AutoCreateDevice { get; set; } = true;
}
