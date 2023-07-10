using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Area MapArea(DbArea dbArea)
	{
		return new Area
		{
			Id = dbArea.Id,
			Name = dbArea.Name,
			Location = dbArea.Location
		};
	}
    
	internal static DbArea MapArea(Area area)
	{
		return new DbArea
		{
			Id = area.Id,
			Name = area.Name,
			Location = area.Location
		};
	}
}