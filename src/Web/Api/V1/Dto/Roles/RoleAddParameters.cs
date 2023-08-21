using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public sealed class RoleAddParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<RoleDto> Roles { get; set; }
}
