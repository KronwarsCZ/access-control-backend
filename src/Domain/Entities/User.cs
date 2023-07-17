using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class User : Entity
{
    public required string Email { get; init; }

    public required UserRole Role { get; init; }

    public required string PasswordHash { get; init; }

    public required bool TwoFactorAuthenticationEnabled { get; init; }
}
