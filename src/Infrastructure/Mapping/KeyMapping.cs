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
            Roles = default,
            CreatedUtc = keyEntity.CreatedUtc,
            UpdatedUtc = keyEntity.UpdatedUtc,
            IsDeleted = keyEntity.IsDeleted
        };
	}
    
	internal static KeyEntity MapKey(Key key)
	{
		return new KeyEntity
        {
            Id = key.Id,
            Name = key.Name,
            Pass = key.Pass,
            CreatedUtc = key.CreatedUtc,
            UpdatedUtc = key.UpdatedUtc,
            IsDeleted = key.IsDeleted
        };
	}
}