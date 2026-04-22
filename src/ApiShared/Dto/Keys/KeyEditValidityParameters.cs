using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public sealed class KeyEditValidityParameters
{
    [Required]
    public Guid KeyId { get; set; }

    public DateTime? ValidityStartUtc { get; set; }

    public DateTime? ValidityEndUtc { get; set; }

    /// When true, ValidityEnd is cleared (no expiry). When false, ValidityEndUtc is applied only if it has a value;
    /// a null ValidityEndUtc means "leave unchanged".
    public bool ClearValidityEnd { get; set; }
}
