using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Roles;

public class GetRoleById
{
    [Required]
    public Guid Id { get; init; }
}
