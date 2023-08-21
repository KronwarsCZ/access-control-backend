using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public class RoleUpdateParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<RoleUpdateDto> Roles { get; set; }
}
