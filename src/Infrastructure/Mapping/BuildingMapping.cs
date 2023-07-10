using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Building MapBuilding(DbBuilding dbBuilding)
	{
		return new Building
		{
			Id = dbBuilding.Id,
			Name = dbBuilding.Name,
			Address = dbBuilding.Address
		};
	}
    
	internal static DbBuilding MapBuilding(Building building)
	{
		return new DbBuilding
		{
			Id = building.Id,
			Name = building.Name,
			Address = building.Address
		};
	}
}