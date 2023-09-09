using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Core.Entities.Accounts;

public class Account : Entity
{
    public required string Email { get; init; }
    
    public required AccountRole Role { get; init; }

    public required string PasswordHash { get; init; }

    public required bool TwoFactorAuthenticationEnabled { get; init; }
    
    public required Owner Owner { get; set; }
    
    public Guid OwnerId { get; }

}
