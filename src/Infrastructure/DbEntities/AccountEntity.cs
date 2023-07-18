using Core.Entities;

namespace Infrastructure.DbEntities;

public class AccountEntity : DbEntity
{
    public required string Email { get; init; }

    public required string Role { get; init; }

    public required string PasswordHash { get; init; }

    public required bool TwoFactorAuthenticationEnabled { get; init; }
    
    public OwnerEntity Owner { get; set; }
}
