using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static Account MapUser(AccountEntity accountEntity)
    {
        return new Account
        {
            Id = accountEntity.Id,
            Email = accountEntity.Email,
            Role = Enum.Parse<AccountRole>(accountEntity.Role),
            PasswordHash = accountEntity.PasswordHash,
            TwoFactorAuthenticationEnabled = accountEntity.TwoFactorAuthenticationEnabled,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false,
            Owner = null
        };
    }

    internal static AccountEntity MapUser(Account account)
    {
        return new AccountEntity
        {
            Id = account.Id,
            Email = account.Email,
            Role = account.Role.ToString(),
            PasswordHash = account.PasswordHash,
            TwoFactorAuthenticationEnabled = account.TwoFactorAuthenticationEnabled,
            CreatedUtc = default,
            UpdatedUtc = default,
            Name = null
        };
    }
}
