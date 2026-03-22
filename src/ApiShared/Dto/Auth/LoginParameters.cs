using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Auth;

public class LoginParameters
{
    [Required]
    public string Email { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}
