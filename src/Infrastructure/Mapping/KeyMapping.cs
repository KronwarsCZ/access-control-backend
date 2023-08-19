using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mappersdasdasd
{
	internal static Key Map(KeyEntity keyEntity)
	{
		return new Key
        {
            Id = keyEntity.Id,
            Name = keyEntity.Name,
            Pass = keyEntity.Pass,
            CreatedUtc = keyEntity.CreatedUtc,
            UpdatedUtc = keyEntity.UpdatedUtc,
            IsDeleted = keyEntity.IsDeleted
        };
	}
    
	internal static KeyEntity Map(Key key)
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