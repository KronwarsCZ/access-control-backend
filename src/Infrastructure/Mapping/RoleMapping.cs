using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Role MapRole(DbRole dbRole)
	{
		return new Role
		{
			Id = dbRole.Id,
			Name = dbRole.Name
		};
	}
    
	internal static DbRole MapRole(Role role)
	{
		return new DbRole
		{
			Id = role.Id,
			Name = role.Name
		};
	}
}