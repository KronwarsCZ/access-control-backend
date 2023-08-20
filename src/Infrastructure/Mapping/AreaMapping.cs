using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Area Map(AreaEntity areaEntity)
	{
		return new Area
        {
            Id = areaEntity.Id,
            Name = areaEntity.Name,
            Location = areaEntity.Location,
            Owner = Map(areaEntity.Owner),
            Buildings = null,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false
        };
	}

	internal static AreaEntity Map(Area area)
	{
		return new AreaEntity
        {
            Id = area.Id,
            Name = area.Name,
            Location = area.Location,
            CreatedUtc = default,
            UpdatedUtc = default
        };
	}
}