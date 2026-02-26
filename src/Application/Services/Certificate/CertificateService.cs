using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Certificate;

/// <summary>
/// Implementation of certificate signing service using .NET cryptography.
/// Supports EC P-256 keys as specified for ESP32 devices.
/// </summary>
public class CertificateService : ICertificateService
{
    private const string EnvCaKey = "CA_KEY";

    private readonly CertificateConfiguration _configuration;
    private readonly ILogger<CertificateService> _logger;
    private readonly string _basePath;
    
    private X509Certificate2? _caCertificate;
    private ECDsa? _caPrivateKey;
    private string? _caCertificatePem;
    private string? _caChainPem;

    public CertificateService(IOptions<CertificateConfiguration> configuration, ILogger<CertificateService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
        _basePath = AppDomain.CurrentDomain.BaseDirectory;
        
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
            // Parse the CSR
            var csr = CertificateRequest.LoadSigningRequestPem(
                csrPem, 
                HashAlgorithmName.SHA256,
                CertificateRequestLoadOptions.Default);

            // Validate the subject contains the expected device ID
            var expectedCn = $"CN={deviceId}";
            if (!csr.SubjectName.Name.Contains(expectedCn, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"CSR subject does not match device ID. Expected: {expectedCn}");
            }

            // Generate certificate
            var notBefore = DateTimeOffset.UtcNow;
            var notAfter = notBefore.AddDays(_configuration.CertificateValidityDays);
            var serialNumber = GenerateSerialNumber();

            // Create the certificate
            using var rsaOrEcdsa = _caPrivateKey ?? throw new InvalidOperationException("CA private key not loaded");
            
            var certificate = csr.Create(
                _caCertificate!.SubjectName,
                X509SignatureGenerator.CreateForECDsa(rsaOrEcdsa),
                notBefore,
                notAfter,
                serialNumber);

            // Export as PEM
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

        // Check for valid PEM markers
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

            // Validate subject contains expected CN
            if (!csr.SubjectName.Name.Contains($"CN={expectedCn}", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("CSR CN mismatch. Expected: {ExpectedCn}, Got: {ActualCn}", 
                    expectedCn, csr.SubjectName.Name);
                return false;
            }

            // Validate key algorithm (should be EC P-256)
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
            var certPath = GetFullPath(_configuration.CaCertificatePath);
            var keyPath = GetFullPath(_configuration.CaPrivateKeyPath);

            if (!File.Exists(certPath))
            {
                _logger.LogWarning("CA certificate not found at {Path}", certPath);
                return;
            }

            if (!File.Exists(keyPath))
            {
                if (!TryWritePrivateKeyFromEnvironment(keyPath))
                {
                    _logger.LogWarning("CA private key not found at {Path}", keyPath);
                    return;
                }
            }

            // Load CA certificate
            _caCertificatePem = File.ReadAllText(certPath);
            _caCertificate = X509Certificate2.CreateFromPem(_caCertificatePem);

            // Load CA private key
            var keyPem = File.ReadAllText(keyPath);
            _caPrivateKey = ECDsa.Create();
            
            if (!string.IsNullOrEmpty(_configuration.CaPrivateKeyPassword))
            {
                _caPrivateKey.ImportFromEncryptedPem(keyPem, _configuration.CaPrivateKeyPassword);
            }
            else
            {
                _caPrivateKey.ImportFromPem(keyPem);
            }

            _logger.LogInformation("CA certificate and key loaded successfully");

            var chainPath = GetFullPath(_configuration.CaChainPath);
            if (File.Exists(chainPath))
            {
                _caChainPem = File.ReadAllText(chainPath);
                _logger.LogInformation("CA chain certificate loaded from {Path}", chainPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load CA certificate and key");
        }
    }

    private bool TryWritePrivateKeyFromEnvironment(string keyPath)
    {
        var rawKey = Environment.GetEnvironmentVariable(EnvCaKey);
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            return false;
        }

        var directory = Path.GetDirectoryName(keyPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var keyPem = DecodePem(rawKey);
        File.WriteAllText(keyPath, keyPem, Encoding.UTF8);
        _logger.LogInformation("CA private key written to {Path} from {EnvVar}", keyPath, EnvCaKey);

        return true;
    }

    private static string DecodePem(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith("-----BEGIN", StringComparison.Ordinal))
        {
            return trimmed;
        }

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(trimmed));
            if (decoded.StartsWith("-----BEGIN", StringComparison.Ordinal))
            {
                return decoded;
            }
        }
        catch (FormatException)
        {
            // Not base64; keep raw value
        }

        return trimmed;
    }

    private string GetFullPath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }
        return Path.Combine(_basePath, configuredPath);
    }

    private static byte[] GenerateSerialNumber()
    {
        var serial = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(serial);
        // Ensure positive number (first byte MSB = 0)
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
