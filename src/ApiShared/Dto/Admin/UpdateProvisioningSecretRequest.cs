using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to set the provisioning secret for the caller's owner.
/// </summary>
public class UpdateProvisioningSecretRequest
{
    /// <summary>
    /// The plaintext secret. Must be at least 16 characters.
    /// </summary>
    [Required]
    [MinLength(16, ErrorMessage = "Secret must be at least 16 characters")]
    [MaxLength(128, ErrorMessage = "Secret cannot exceed 128 characters")]
    public string NewSecret { get; init; } = null!;

    /// <summary>
    /// Optional friendly label (e.g. "Factory Floor A").
    /// </summary>
    [MaxLength(128)]
    public string? Label { get; init; }
}
