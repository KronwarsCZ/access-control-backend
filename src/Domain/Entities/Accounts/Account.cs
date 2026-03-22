using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Core.Entities.Accounts;

public class Account : OwnedEntity
{
    private Account()
    {
    }

    public string Email { get; set; } = null!;

    public AccountRole Role { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool TwoFactorAuthenticationEnabled { get; set; }

    public int TokenVersion { get; set; }

    public static Account Create(Owner owner, string email, AccountRole role, string passwordHash, bool twofactorAuthEnabled = false)
    {
        var account = CreateEntity<Account>();
        account.Owner = owner;
        account.OwnerId = owner.Id;
        account.Email = email;
        account.Role = role;
        account.PasswordHash = passwordHash;
        account.TwoFactorAuthenticationEnabled = twofactorAuthEnabled;
        return account;
    }
}
