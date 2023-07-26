using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class KeyDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Pass { get; set; }
}
