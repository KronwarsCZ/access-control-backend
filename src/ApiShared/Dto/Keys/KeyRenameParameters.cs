using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public sealed class KeyRenameParameters
{
    [Required]
    public Guid KeyId { get; set; }

    [Required]
    public string NewName { get; set; } = null!;
}
