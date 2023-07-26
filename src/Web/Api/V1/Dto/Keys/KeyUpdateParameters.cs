using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class KeyUpdateParameters
{
    // rekneme, ze chceme davat najednou max 100 klicu
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<KeyUpdateDto> Keys { get; set; }
}
