using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Serilog;

namespace AccessControl.Web.StartupMethods;

/// <summary>
/// Configures Kestrel with a server TLS certificate and optional mTLS validation.
///
/// Required environment variables (app refuses to start without them):
///   SERVER_CERT – PEM of the server TLS certificate (SANs should cover localhost + LAN IP)
///   SERVER_KEY  – PEM of the server TLS private key
///   ROOT_CERT   – PEM of the Root CA certificate (used to validate mTLS client certs)
///
/// All values accept raw PEM or base64-encoded PEM.
/// </summary>
public static class KestrelTlsConfigurator
{
    private const string EnvServerCert = "SERVER_CERT";
    private const string EnvServerKey = "SERVER_KEY";
    private const string EnvCaChain = "ROOT_CERT";
    private const string EnvCaCert = "CA_CERT";

    public static void ConfigureKestrelTls(this WebApplicationBuilder builder)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            Log.Information("Test environment detected — skipping Kestrel TLS configuration");
            return;
        }

        var serverCertPem = RequireEnvVar(EnvServerCert);
        var serverKeyPem = RequireEnvVar(EnvServerKey);
        var rootCaPem = RequireEnvVar(EnvCaChain);

        var serverCert = LoadServerCertificate(serverCertPem, serverKeyPem);
        var rootCa = LoadCertificateFromPem(rootCaPem, "Root CA");

        X509Certificate2? intermediateCa = null;
        var intermediateCaPem = Environment.GetEnvironmentVariable(EnvCaCert);
        if (!string.IsNullOrWhiteSpace(intermediateCaPem))
        {
            intermediateCa = LoadCertificateFromPem(DecodePem(intermediateCaPem), "Intermediate CA");
            LogCertificateDetails("Intermediate CA", intermediateCa);
        }

        LogCertificateDetails("Server", serverCert);
        LogCertificateDetails("Root CA", rootCa);

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.ServerCertificate = serverCert;
                httpsOptions.ClientCertificateMode = ClientCertificateMode.DelayCertificate;

                httpsOptions.OnAuthenticate = (context, sslOptions) =>
                {
                    Log.Debug("TLS handshake starting — remote: {Remote}, protocol: {Protocol}",
                        context.RemoteEndPoint, sslOptions.EnabledSslProtocols);
                };

                httpsOptions.ClientCertificateValidation = (clientCert, chain, errors) =>
                {
                    Log.Debug("Client cert validation — Subject: {Subject}, Issuer: {Issuer}, " +
                              "Thumbprint: {Thumbprint}, SslErrors: {SslErrors}",
                        clientCert.Subject, clientCert.Issuer,
                        clientCert.Thumbprint, errors);

                    if (chain == null)
                    {
                        Log.Warning("Client cert rejected — X509Chain was null");
                        return false;
                    }

                    chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                    chain.ChainPolicy.CustomTrustStore.Add(rootCa);
                    if (intermediateCa != null)
                        chain.ChainPolicy.ExtraStore.Add(intermediateCa);
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    var isValid = chain.Build(clientCert);
                    if (!isValid)
                    {
                        Log.Warning(
                            "Client cert chain build failed for {Subject}: {Errors}",
                            clientCert.Subject,
                            string.Join("; ", chain.ChainStatus.Select(s =>
                                $"{s.Status}: {s.StatusInformation}")));
                    }
                    else
                    {
                        Log.Debug("Client cert accepted — {Subject}", clientCert.Subject);
                    }

                    return isValid;
                };
            });
        });

        Log.Information("Kestrel TLS configured — server: {ServerSubject}, mTLS root: {RootSubject}",
            serverCert.Subject, rootCa.Subject);
    }

    private static void LogCertificateDetails(string label, X509Certificate2 cert)
    {
        Log.Information("{Label} cert — Subject: {Subject}, Issuer: {Issuer}, " +
                        "Thumbprint: {Thumbprint}, NotBefore: {NotBefore}, NotAfter: {NotAfter}, " +
                        "KeyAlgorithm: {KeyAlg}, HasPrivateKey: {HasKey}",
            label, cert.Subject, cert.Issuer,
            cert.Thumbprint, cert.NotBefore, cert.NotAfter,
            cert.GetKeyAlgorithm(), cert.HasPrivateKey);

        try
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext is X509SubjectAlternativeNameExtension san)
                    Log.Information("{Label} cert SANs: {SANs}", label, san.Format(multiLine: false));
                if (ext is X509EnhancedKeyUsageExtension eku)
                    Log.Information("{Label} cert EKU: {EKU}", label, eku.Format(multiLine: false));
                if (ext is X509KeyUsageExtension ku)
                    Log.Information("{Label} cert KeyUsage: {KU}", label, ku.Format(multiLine: false));
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not enumerate {Label} cert extensions", label);
        }
    }

    private static string RequireEnvVar(string name)
    {
        var raw = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException(
                $"Required environment variable {name} is not set. " +
                "The application cannot start without certificate material.");

        return DecodePem(raw);
    }

    private static X509Certificate2 LoadServerCertificate(string certPem, string keyPem)
    {
        try
        {
            var cert = X509Certificate2.CreateFromPem(certPem, keyPem);
            cert = new X509Certificate2(
                cert.Export(X509ContentType.Pfx),
                (string?)null,
                X509KeyStorageFlags.Exportable);
            return cert;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load server TLS certificate from {EnvServerCert}/{EnvServerKey}", ex);
        }
    }

    private static X509Certificate2 LoadCertificateFromPem(string pem, string label)
    {
        try
        {
            return X509Certificate2.CreateFromPem(pem);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse {label} PEM from {EnvCaChain}", ex);
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
            // Not base64
        }

        return trimmed;
    }
}
