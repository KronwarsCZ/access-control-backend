using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class RoleDto
{
    [Required]
    public string Name { get; set; }
}
