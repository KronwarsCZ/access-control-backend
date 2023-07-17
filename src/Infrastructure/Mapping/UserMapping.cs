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
            Role = Enum.Parse<UserRole>(userEntity.Role),
            PasswordHash = userEntity.PasswordHash,
            TwoFactorAuthenticationEnabled = userEntity.TwoFactorAuthenticationEnabled,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false
        };
    }

    internal static UserEntity MapUser(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            PasswordHash = user.PasswordHash,
            TwoFactorAuthenticationEnabled = user.TwoFactorAuthenticationEnabled,
            CreatedUtc = default,
            UpdatedUtc = default,
            Name = null
        };
    }
}
