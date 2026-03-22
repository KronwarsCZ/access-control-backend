using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.IoT;

/// <summary>
/// Request for renewing an existing device certificate.
/// Requires mTLS authentication.
/// </summary>
public class RenewalRequest
{
    /// <summary>
    /// Device identifier - must match the CN in the mTLS client certificate.
    /// </summary>
    [Required]
    [RegularExpression(@"^DEV-[A-F0-9]{12}$", ErrorMessage = "Device ID must be in format DEV-XXXXXXXXXXXX (12 hex chars)")]
    public string DeviceId { get; init; } = null!;

    /// <summary>
    /// New PEM-encoded PKCS#10 CSR.
    /// </summary>
    [Required]
    public string Csr { get; init; } = null!;

    /// <summary>
    /// Must be true for renewal requests.
    /// </summary>
    [Required]
    public bool Renewal { get; init; } = true;
}
