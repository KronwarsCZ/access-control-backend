using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Role MapRole(RoleEntity dbRole)
	{
		return new Role
		{
			Id = dbRole.Id,
			Name = dbRole.Name
		};
	}
    
	internal static RoleEntity MapRole(Role role)
	{
		return new RoleEntity
		{
			Id = role.Id,
			Name = role.Name
		};
	}
}