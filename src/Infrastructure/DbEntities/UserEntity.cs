using Core.Entities;

namespace Infrastructure.DbEntities;

public class UserEntity
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Role { get; init; }

    public required string PasswordHash { get; init; }

    public required bool TwoFactorAuthenticationEnabled { get; init; }
}
