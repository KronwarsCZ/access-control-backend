using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Certificate;

/// <summary>
/// Certificate signing service using .NET cryptography.
/// Loads intermediate CA cert + key from environment variables (CA_CERT, CA_KEY).
/// Optionally loads the root CA chain from ROOT_CERT.
/// </summary>
public class CertificateService : ICertificateService
{
    private const string EnvCaCert = "CA_CERT";
    private const string EnvCaKey = "CA_KEY";
    private const string EnvCaChain = "ROOT_CERT";

    private readonly CertificateConfiguration _configuration;
    private readonly ILogger<CertificateService> _logger;

    private X509Certificate2? _caCertificate;
    private ECDsa? _caPrivateKey;
    private string? _caCertificatePem;
    private string? _caChainPem;

    public CertificateService(IOptions<CertificateConfiguration> configuration, ILogger<CertificateService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;

        LoadCaCertificateAndKey();
    }

    /// <inheritdoc />
    public async Task<string> SignCsrAsync(string csrPem, string deviceId, CancellationToken cancellationToken)
    {
        if (!IsConfigured())
        {
            throw new InvalidOperationException("Certificate service is not properly configured");
        }

        if (!ValidateCsr(csrPem, deviceId))
        {
            throw new ArgumentException("Invalid CSR", nameof(csrPem));
        }

        try
        {
            var csr = CertificateRequest.LoadSigningRequestPem(
                csrPem,
                HashAlgorithmName.SHA256,
                CertificateRequestLoadOptions.Default);

            var expectedCn = $"CN={deviceId}";
            if (!csr.SubjectName.Name.Contains(expectedCn, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"CSR subject does not match device ID. Expected: {expectedCn}");
            }

            var notBefore = DateTimeOffset.UtcNow;
            var notAfter = notBefore.AddDays(_configuration.CertificateValidityDays);
            var serialNumber = GenerateSerialNumber();

            using var ecdsa = _caPrivateKey ?? throw new InvalidOperationException("CA private key not loaded");

            var certificate = csr.Create(
                _caCertificate!.SubjectName,
                X509SignatureGenerator.CreateForECDsa(ecdsa),
                notBefore,
                notAfter,
                serialNumber);

            var certPem = ExportCertificateToPem(certificate);

            _logger.LogInformation("Successfully signed certificate for device {DeviceId}", deviceId);

            return certPem;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error signing CSR for device {DeviceId}", deviceId);
            throw new InvalidOperationException("Failed to sign certificate", ex);
        }
    }

    /// <inheritdoc />
    public string GetCaCertificate()
    {
        if (string.IsNullOrEmpty(_caCertificatePem))
        {
            throw new InvalidOperationException("CA certificate not loaded");
        }

        if (!string.IsNullOrEmpty(_caChainPem))
            return _caCertificatePem + _caChainPem;

        return _caCertificatePem;
    }

    /// <inheritdoc />
    public bool ValidateCsr(string csrPem, string expectedCn)
    {
        if (string.IsNullOrWhiteSpace(csrPem))
        {
            _logger.LogWarning("Empty CSR provided");
            return false;
        }

        if (!csrPem.Contains("-----BEGIN CERTIFICATE REQUEST-----") ||
            !csrPem.Contains("-----END CERTIFICATE REQUEST-----"))
        {
            _logger.LogWarning("CSR missing PEM markers");
            return false;
        }

        try
        {
            var csr = CertificateRequest.LoadSigningRequestPem(
                csrPem,
                HashAlgorithmName.SHA256,
                CertificateRequestLoadOptions.Default);

            if (!csr.SubjectName.Name.Contains($"CN={expectedCn}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("CSR CN mismatch. Expected: {ExpectedCn}, Got: {ActualCn}",
                    expectedCn, csr.SubjectName.Name);
                return false;
            }

            var publicKey = csr.PublicKey;
            if (publicKey.Oid.Value != "1.2.840.10045.2.1") // EC key OID
            {
                _logger.LogWarning("CSR uses unsupported key algorithm: {Oid}", publicKey.Oid.Value);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse CSR");
            return false;
        }
    }

    /// <inheritdoc />
    public bool ValidateProvisioningSecret(string secret)
    {
        if (string.IsNullOrEmpty(_configuration.ProvisioningSecret))
        {
            _logger.LogError("Provisioning secret not configured!");
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes(_configuration.ProvisioningSecret));
    }

    /// <inheritdoc />
    public bool IsConfigured()
    {
        return _caCertificate != null && _caPrivateKey != null && !string.IsNullOrEmpty(_caCertificatePem);
    }

    private void LoadCaCertificateAndKey()
    {
        try
        {
            var rawCert = Environment.GetEnvironmentVariable(EnvCaCert);
            var rawKey = Environment.GetEnvironmentVariable(EnvCaKey);

            if (string.IsNullOrWhiteSpace(rawCert))
            {
                _logger.LogWarning("Environment variable {EnvVar} is not set — CA certificate not loaded", EnvCaCert);
                return;
            }

            if (string.IsNullOrWhiteSpace(rawKey))
            {
                _logger.LogWarning("Environment variable {EnvVar} is not set — CA private key not loaded", EnvCaKey);
                return;
            }

            _caCertificatePem = DecodePem(rawCert);
            _caCertificate = X509Certificate2.CreateFromPem(_caCertificatePem);

            var keyPem = DecodePem(rawKey);
            _caPrivateKey = ECDsa.Create();

            if (!string.IsNullOrEmpty(_configuration.CaPrivateKeyPassword))
            {
                _caPrivateKey.ImportFromEncryptedPem(keyPem, _configuration.CaPrivateKeyPassword);
            }
            else
            {
                _caPrivateKey.ImportFromPem(keyPem);
            }

            _logger.LogInformation("CA certificate and key loaded from environment variables");

            var rawChain = Environment.GetEnvironmentVariable(EnvCaChain);
            if (!string.IsNullOrWhiteSpace(rawChain))
            {
                _caChainPem = DecodePem(rawChain);
                _logger.LogInformation("CA chain loaded from {EnvVar}", EnvCaChain);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load CA certificate and key from environment variables");
        }
    }

    private static string DecodePem(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith("-----BEGIN", StringComparison.Ordinal))
            return trimmed;

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(trimmed));
            if (decoded.StartsWith("-----BEGIN", StringComparison.Ordinal))
                return decoded;
        }
        catch (FormatException)
        {
            // Not base64; keep raw value
        }

        return trimmed;
    }

    private static byte[] GenerateSerialNumber()
    {
        var serial = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(serial);
        serial[0] &= 0x7F;
        return serial;
    }

    private static string ExportCertificateToPem(X509Certificate2 certificate)
    {
        var builder = new StringBuilder();
        builder.AppendLine("-----BEGIN CERTIFICATE-----");
        builder.AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE-----");
        return builder.ToString();
    }
}
