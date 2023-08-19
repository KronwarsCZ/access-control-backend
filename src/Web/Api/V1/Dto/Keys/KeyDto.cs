using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Keys;

public sealed class KeyDto : Dto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Pass { get; set; }
}
