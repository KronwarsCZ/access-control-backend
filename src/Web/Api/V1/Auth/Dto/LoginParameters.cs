using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Auth.Dto;

public class LoginParameters
{
    [Required]
    public string Email { get; init; }
    [Required]
    public string Password { get; init; }
}
