namespace Application.Services.Certificate;

/// <summary>
/// Configuration for certificate management and signing.
/// All certificate material (PEM) is loaded from environment variables at startup.
/// </summary>
public class CertificateConfiguration
{
    public const string SectionName = "Certificate";

    /// <summary>
    /// Pre-shared secret for device provisioning.
    /// Must be kept secure and rotated periodically.
    /// </summary>
    public string ProvisioningSecret { get; set; } = string.Empty;

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
