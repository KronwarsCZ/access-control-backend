using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class RoleUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }
}
