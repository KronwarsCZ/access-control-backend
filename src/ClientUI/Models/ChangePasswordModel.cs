using System.ComponentModel.DataAnnotations;

namespace DemoRegistrationEncryptionUsingRandomNumberAndSalt.Models;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$",
        ErrorMessage = "Password must contain an uppercase letter, lowercase letter, digit, and special character")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Please confirm your new password")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string? ConfirmNewPassword { get; set; }
}
