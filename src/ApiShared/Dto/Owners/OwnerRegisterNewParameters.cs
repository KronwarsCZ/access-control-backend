using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Owners;

public class RegisterNewOwnerParameters
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Email { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}
