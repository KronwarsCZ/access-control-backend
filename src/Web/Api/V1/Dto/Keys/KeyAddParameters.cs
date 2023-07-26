using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class KeyAddParameters
{
    // rekneme, ze chceme davat najednou max 100 klicu
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<KeyDto> Keys { get; set; }
}
