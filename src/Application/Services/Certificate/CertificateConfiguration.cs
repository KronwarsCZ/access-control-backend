namespace Application.Services.Certificate;

/// <summary>
/// Configuration for certificate management and signing.
/// </summary>
public class CertificateConfiguration
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Certificate";

    /// <summary>
    /// Pre-shared secret for device provisioning.
    /// Must be kept secure and rotated periodically.
    /// </summary>
    public string ProvisioningSecret { get; set; } = string.Empty;

    /// <summary>
    /// Path to the CA private key file (PEM format).
    /// Used for signing device certificates.
    /// </summary>
    public string CaPrivateKeyPath { get; set; } = "certs/ca.key";

    /// <summary>
    /// Path to the CA certificate file (PEM format).
    /// </summary>
    public string CaCertificatePath { get; set; } = "certs/ca.crt";

    /// <summary>
    /// Path to the CA chain file (PEM format).
    /// Contains the root/intermediate CA that signed the signing CA.
    /// </summary>
    public string CaChainPath { get; set; } = "certs/ca-chain.crt";

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
    /// Issuer name for certificates.
    /// </summary>
    public string IssuerName { get; set; } = "CN=AccessControl CA, O=AccessControl, C=CZ";

    /// <summary>
    /// Minimum days before expiry when renewal is allowed.
    /// </summary>
    public int RenewalWindowDays { get; set; } = 30;

    /// <summary>
    /// Default owner ID for auto-provisioned devices.
    /// If not set, devices will be assigned to the first owner found.
    /// </summary>
    public Guid? DefaultOwnerId { get; set; }

    /// <summary>
    /// Whether to automatically create a Device entity when provisioning.
    /// Default: true.
    /// </summary>
    public bool AutoCreateDevice { get; set; } = true;
}
