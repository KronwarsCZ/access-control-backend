using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Owners;

public class RegisterNewOwnerParameters
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Email { get; init; }
    [Required]
    public string Password { get; init; }
}
