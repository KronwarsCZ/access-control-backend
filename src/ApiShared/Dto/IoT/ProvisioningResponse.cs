namespace ApiShared.Dto.IoT;

/// <summary>
/// Response from device provisioning.
/// Contains the signed client certificate and CA certificate.
/// </summary>
public class ProvisioningResponse
{
    /// <summary>
    /// PEM-encoded X.509 certificate signed by the CA.
    /// </summary>
    public string ClientCert { get; init; } = null!;

    /// <summary>
    /// PEM-encoded CA certificate for chain validation.
    /// </summary>
    public string CaCert { get; init; } = null!;
}
