namespace ApiShared.Dto.IoT;

/// <summary>
/// Response from certificate renewal.
/// Contains the new signed client certificate.
/// </summary>
public class RenewalResponse
{
    /// <summary>
    /// PEM-encoded X.509 certificate signed by the CA.
    /// </summary>
    public required string ClientCert { get; init; }
}
