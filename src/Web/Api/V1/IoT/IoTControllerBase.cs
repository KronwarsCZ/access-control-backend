using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1.IoT;

/// <summary>
/// Base class for IoT controllers that require client certificate authentication.
/// In production (App Service), reads the client cert from the X-ARR-ClientCert header
/// and validates the chain against ROOT_CERT (and optionally CA_CERT).
/// In other environments, reads from the TLS connection (Kestrel mTLS handles validation).
/// </summary>
public abstract class IoTControllerBase : ControllerBase
{
    private static readonly Lazy<X509Certificate2?> RootCa = new(() => LoadCertFromEnv("ROOT_CERT"));
    private static readonly Lazy<X509Certificate2?> IntermediateCa = new(() => LoadCertFromEnv("CA_CERT"));

    protected async Task<X509Certificate2?> GetClientCertificateAsync(
        IWebHostEnvironment environment,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        X509Certificate2? cert;

        if (environment.IsProduction())
        {
            if (!Request.Headers.TryGetValue("X-ARR-ClientCert", out var certHeader)
                || string.IsNullOrEmpty(certHeader))
            {
                logger.LogDebug("No X-ARR-ClientCert header present");
                return null;
            }

            try
            {
                var certBytes = Convert.FromBase64String(certHeader!);
                cert = new X509Certificate2(certBytes);
                logger.LogDebug("Client certificate from X-ARR-ClientCert — Subject: {Subject}, Issuer: {Issuer}",
                    cert.Subject, cert.Issuer);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to parse client certificate from X-ARR-ClientCert header");
                return null;
            }
        }
        else
        {
            cert = await HttpContext.Connection.GetClientCertificateAsync(cancellationToken);
        }

        if (cert == null)
            return null;

        return !ValidateClientCertificateChain(cert, logger) ? null : cert;
    }

    private static bool ValidateClientCertificateChain(X509Certificate2 clientCert, ILogger logger)
    {
        var rootCa = RootCa.Value;
        if (rootCa == null)
        {
            logger.LogError("ROOT_CERT not configured — cannot validate client certificate chain");
            return false;
        }

        using var chain = new X509Chain();
        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        chain.ChainPolicy.CustomTrustStore.Add(rootCa);

        var intermediate = IntermediateCa.Value;
        if (intermediate != null)
            chain.ChainPolicy.ExtraStore.Add(intermediate);

        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        var isValid = chain.Build(clientCert);
        if (!isValid)
        {
            logger.LogWarning("Client certificate chain validation failed for {Subject}: {Errors}",
                clientCert.Subject,
                string.Join("; ", chain.ChainStatus.Select(s => $"{s.Status}: {s.StatusInformation}")));
        }
        else
        {
            logger.LogDebug("Client certificate chain validated — {Subject}", clientCert.Subject);
        }

        return isValid;
    }

    private static X509Certificate2? LoadCertFromEnv(string envVar)
    {
        var raw = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var pem = DecodePem(raw);
        return X509Certificate2.CreateFromPem(pem);
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
        catch (FormatException) { }

        return trimmed;
    }

    protected static string? ExtractCnFromSubject(string subject)
    {
        var parts = subject.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed[3..];
            }
        }
        return null;
    }
}