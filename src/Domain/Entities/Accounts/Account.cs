using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Core.Entities.Accounts;

public class Account : Entity
{
    private Account()
    {
    }

    public string Email { get; set; }
    
    public AccountRole Role { get; set; }

    public string PasswordHash { get; set; }

    public bool TwoFactorAuthenticationEnabled { get; set; }
    
    public Owner Owner { get; set; }
    
    public Guid OwnerId { get; }

    public string SessionToken { get; set; }

    public static Account Create(Owner owner, string email, AccountRole role, string passwordHash, bool twofactorAuthEnabled = false)
    {
        var account = CreateEntity<Account>();
        account.Owner = owner;
        account.Email = email;
        account.Role = role;
        account.PasswordHash = passwordHash;
        account.TwoFactorAuthenticationEnabled = twofactorAuthEnabled;
        return account;
    }
}
