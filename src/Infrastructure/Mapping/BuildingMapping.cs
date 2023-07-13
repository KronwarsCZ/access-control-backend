using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Building MapBuilding(BuildingEntity buildingEntity)
	{
		return new Building
		{
			Id = buildingEntity.Id,
			Name = buildingEntity.Name,
			Address = buildingEntity.Address
		};
	}
    
	internal static BuildingEntity MapBuilding(Building building)
	{
		return new BuildingEntity
		{
			Id = building.Id,
			Name = building.Name,
			Address = building.Address
		};
	}
}