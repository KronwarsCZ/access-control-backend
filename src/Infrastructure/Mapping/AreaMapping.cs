using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Area MapArea(AreaEntity areaEntity)
	{
		return new Area
		{
			Id = areaEntity.Id,
			Name = areaEntity.Name,
			Location = areaEntity.Location
		};
	}

	internal static AreaEntity MapArea(Area area)
	{
		return new AreaEntity
		{
			Id = area.Id,
			Name = area.Name,
			Location = area.Location
		};
	}
}