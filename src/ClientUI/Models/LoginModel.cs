using System.ComponentModel.DataAnnotations;

namespace AccessControl.ClientUI.Models
{
    public class LoginModel
    {
        [EmailAddress, Required]
        public string? Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
