using System.Text;
using Application.Services.Certificate;
using Microsoft.Extensions.Options;
using Serilog;

namespace AccessControl.Web.StartupMethods;

/// <summary>
/// Writes CA certificate material from environment variables to disk
/// so CertificateService can load them for signing device CSRs.
///
/// Environment variables:
///   CA_CERT  – PEM (or base64-encoded PEM) of the signing CA certificate
///   CA_KEY   – PEM (or base64-encoded PEM) of the signing CA private key
///   CA_CHAIN – PEM (or base64-encoded PEM) of the root/intermediate CA that signed the signing CA
/// </summary>
public static class CertificateInitializer
{
    private const string EnvCaCert = "CA_CERT";
    private const string EnvCaKey = "CA_KEY";
    private const string EnvCaChain = "CA_CHAIN";

    public static void EnsureCaCertificateExists(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<IOptions<CertificateConfiguration>>().Value;
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var certPath = ResolvePath(basePath, config.CaCertificatePath);
        var keyPath = ResolvePath(basePath, config.CaPrivateKeyPath);
        var chainPath = ResolvePath(basePath, config.CaChainPath);

        var written = false;

        written |= WriteFromEnvVar(EnvCaCert, certPath, "CA certificate");
        written |= WriteFromEnvVar(EnvCaKey, keyPath, "CA private key");
        written |= WriteFromEnvVar(EnvCaChain, chainPath, "CA chain");

        if (written)
        {
            Log.Information("Certificate material written to disk from environment variables");
        }

        if (!File.Exists(certPath))
            Log.Warning("CA certificate not found at {Path} — set the {EnvVar} environment variable", certPath, EnvCaCert);

        if (!File.Exists(keyPath))
            Log.Warning("CA private key not found at {Path} — set the {EnvVar} environment variable", keyPath, EnvCaKey);
    }

    /// <returns>true if the file was written</returns>
    private static bool WriteFromEnvVar(string envVar, string filePath, string label)
    {
        if (File.Exists(filePath))
        {
            Log.Information("{Label} already exists at {Path}, skipping env var", label, filePath);
            return false;
        }

        var raw = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var pem = DecodePem(raw);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filePath, pem, Encoding.UTF8);
        Log.Information("{Label} written to {Path} from env var {EnvVar}", label, filePath, envVar);
        return true;
    }

    /// <summary>
    /// Accepts raw PEM or base64-encoded PEM (for environments that
    /// don't handle multi-line env vars well).
    /// </summary>
    private static string DecodePem(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith("-----BEGIN"))
            return trimmed;

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(trimmed));
            if (decoded.StartsWith("-----BEGIN"))
                return decoded;
        }
        catch (FormatException)
        {
            // Not valid base64 — fall through
        }

        return trimmed;
    }

    private static string ResolvePath(string basePath, string configuredPath)
    {
        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(basePath, configuredPath);
    }
}
