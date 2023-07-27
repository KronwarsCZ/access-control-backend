using Core.Entities;
using Framework.Extensions;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static Account MapAccount(AccountEntity accountEntity)
    {
        return new Account
        {
            Id = accountEntity.Id,
            Email = accountEntity.Email,
            Role = accountEntity.Role,
            PasswordHash = accountEntity.PasswordHash,
            TwoFactorAuthenticationEnabled = accountEntity.TwoFactorAuthenticationEnabled,
            CreatedUtc = accountEntity.CreatedUtc,
            UpdatedUtc = accountEntity.UpdatedUtc,
            IsDeleted = accountEntity.IsDeleted,
            Owner = accountEntity.Owner.MapRef(MapOwner)
        };
    }

    internal static AccountEntity MapAccount(Account account)
    {
        return new AccountEntity
        {
            Id = account.Id,
            Email = account.Email,
            Role = account.Role,
            PasswordHash = account.PasswordHash,
            TwoFactorAuthenticationEnabled = account.TwoFactorAuthenticationEnabled,
            CreatedUtc = account.CreatedUtc,
            UpdatedUtc = account.UpdatedUtc,
            IsDeleted = account.IsDeleted,
            Owner = account.Owner.MapRef(MapOwner),
            OwnerId = account.OwnerId
        };
    }
}
