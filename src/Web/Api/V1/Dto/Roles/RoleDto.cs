using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class RoleDto : Dto
{
    [Required]
    public string Name { get; set; }
}
