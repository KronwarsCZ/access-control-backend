using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public class RoleUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }
}
