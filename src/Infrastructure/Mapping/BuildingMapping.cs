using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mappersdasdasd
{
	internal static Building Map(BuildingEntity buildingEntity)
	{
		return new Building
        {
            Id = buildingEntity.Id,
            Name = buildingEntity.Name,
            Address = buildingEntity.Address,
            Area = null,
            Roles = null,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false
        };
	}
    
	internal static BuildingEntity Map(Building building)
	{
		return new BuildingEntity
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address,
            CreatedUtc = default,
            UpdatedUtc = default
        };
	}
}