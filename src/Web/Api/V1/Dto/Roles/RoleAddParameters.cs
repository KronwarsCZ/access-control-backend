using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class RoleAddParameters
{
    [Required]
    public string Name { get; set; }
}
