using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to update the provisioning secret.
/// </summary>
public class UpdateProvisioningSecretRequest
{
    /// <summary>
    /// New provisioning secret.
    /// Must be at least 16 characters for security.
    /// </summary>
    [Required]
    [MinLength(16, ErrorMessage = "Secret must be at least 16 characters")]
    [MaxLength(128, ErrorMessage = "Secret cannot exceed 128 characters")]
    public required string NewSecret { get; init; }
}
