namespace Infrastructure.DbEntities;

public class UserEntity
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required bool TwoFactorAuthenticationEnabled { get; set; }
}
