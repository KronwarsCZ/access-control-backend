using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Role Map(RoleEntity dbRole)
	{
		return new Role
        {
            Id = dbRole.Id,
            Name = dbRole.Name,
            /*Devices = null,
            Keys = null,
            Buildings = null,*/
            CreatedUtc = dbRole.CreatedUtc,
            UpdatedUtc = dbRole.UpdatedUtc,
            IsDeleted = dbRole.IsDeleted
        };
	}
    
	internal static RoleEntity Map(Role role)
	{
		return new RoleEntity
        {
            Id = role.Id,
            Name = role.Name,
            /*Devices = null,
            Keys = null,
            Buildings = null,*/
            CreatedUtc = role.CreatedUtc,
            UpdatedUtc = role.UpdatedUtc,
            IsDeleted = role.IsDeleted
        };
	}
}