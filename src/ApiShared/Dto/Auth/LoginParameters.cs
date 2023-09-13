using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Auth;

public class LoginParameters
{
    [Required]
    public string Email { get; init; }
    [Required]
    public string Password { get; init; }
}
