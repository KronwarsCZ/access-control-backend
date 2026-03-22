using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Auth;

public class ChangePasswordParameters
{
    [Required]
    public string CurrentPassword { get; init; } = null!;
    [Required]
    public string NewPassword { get; init; } = null!;
}
