using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Roles;

public sealed class RoleRenameParameters
{
    [Required]
    public Guid RoleId { get; set; }

    [Required]
    public string NewName { get; set; } = null!;
}
