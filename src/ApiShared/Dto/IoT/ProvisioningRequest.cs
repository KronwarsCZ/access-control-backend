using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.IoT;

/// <summary>
/// Request for provisioning a new IoT device.
/// Contains the CSR and device identification.
/// </summary>
public class ProvisioningRequest
{
    /// <summary>
    /// Unique device identifier (used as CN in CSR).
    /// Format: DEV-{MAC_ADDRESS} where MAC is uppercase hex without colons.
    /// Example: DEV-AABBCCDDEEFF
    /// </summary>
    [Required]
    [RegularExpression(@"^DEV-[A-F0-9]{12}$", ErrorMessage = "Device ID must be in format DEV-XXXXXXXXXXXX (12 hex chars)")]
    public string DeviceId { get; init; } = null!;

    /// <summary>
    /// PEM-encoded PKCS#10 CSR (EC P-256 key).
    /// Must contain BEGIN/END CERTIFICATE REQUEST markers.
    /// </summary>
    [Required]
    public string Csr { get; init; } = null!;

    /// <summary>
    /// Pre-shared secret for authentication during initial provisioning.
    /// </summary>
    [Required]
    [MinLength(16, ErrorMessage = "Shared secret must be at least 16 characters")]
    public string SharedSecret { get; init; } = null!;
}
