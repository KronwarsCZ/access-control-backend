using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Owner MapOwner(OwnerEntity dbOwner)
	{
		return new Owner
		{
			Id = dbOwner.Id,
			Name = dbOwner.Name
			
		};
	}
    
	internal static OwnerEntity MapOwner(Owner owner)
	{
		return new OwnerEntity
		{
			Id = owner.Id,
			Name = owner.Name
		};
	}
}