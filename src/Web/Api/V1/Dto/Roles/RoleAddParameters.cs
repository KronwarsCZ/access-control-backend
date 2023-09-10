using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public sealed class RoleAddParameters
{
    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public string Name { get; set; }
}
