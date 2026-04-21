using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public sealed class KeyEditValidityParameters
{
    [Required]
    public Guid KeyId { get; set; }

    public DateTime? ValidityStartUtc { get; set; }

    public DateTime? ValidityEndUtc { get; set; }
}
