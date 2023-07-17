using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Key MapKey(KeyEntity keyEntity)
	{
		return new Key
        {
            Id = keyEntity.Id,
            Name = keyEntity.Name,
            Pass = keyEntity.Pass,
            Roles = null,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false
        };
	}
    
	internal static KeyEntity MapKey(Key key)
	{
		return new KeyEntity
        {
            Id = key.Id,
            Name = key.Name,
            Pass = key.Pass,
            CreatedUtc = default,
            UpdatedUtc = default
        };
	}
}