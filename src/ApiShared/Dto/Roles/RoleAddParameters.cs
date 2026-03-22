using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Roles;

public sealed class RoleAddParameters
{
    [Required]
    public string Name { get; set; } = null!;
}
