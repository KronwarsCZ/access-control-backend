using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Owner MapOwner(DbOwner dbOwner)
	{
		return new Owner
		{
			Id = dbOwner.Id,
			Name = dbOwner.Name
			
		};
	}
    
	internal static DbOwner MapOwner(Owner owner)
	{
		return new DbOwner
		{
			Id = owner.Id,
			Name = owner.Name
		};
	}
}