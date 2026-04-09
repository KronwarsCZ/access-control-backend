using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Accounts;

public class CreateAccountParameters
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public bool IsAdmin { get; set; } = false;
}
