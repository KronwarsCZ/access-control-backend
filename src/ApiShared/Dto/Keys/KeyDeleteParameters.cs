using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class KeyDeleteParameters
{
    [Required]
    public Guid KeyId { get; init; }
}
