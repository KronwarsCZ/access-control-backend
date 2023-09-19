using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class GetKeyByIdParameters
{
    [Required]
    public Guid Id { get; set; }
}
