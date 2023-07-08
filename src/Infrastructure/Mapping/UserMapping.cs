using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static User MapUser(UserEntity userEntity)
    {
        return new User
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            PasswordHash = userEntity.PasswordHash,
            TwoFactorAuthenticationEnabled = userEntity.TwoFactorAuthenticationEnabled
        };
    }

    internal static UserEntity MapUser(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            TwoFactorAuthenticationEnabled = user.TwoFactorAuthenticationEnabled
        };
    }
}
