using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public sealed class RoleDto
{
    [Required]
    public string Name { get; set; }
}
