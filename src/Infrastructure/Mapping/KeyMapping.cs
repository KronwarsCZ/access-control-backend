using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
	internal static Key MapKey(DbKey dbKey)
	{
		return new Key
		{
			Id = dbKey.Id,
			Name = dbKey.Name,
			Pass = dbKey.Pass
		};
	}
    
	internal static DbKey MapKey(Key key)
	{
		return new DbKey
		{
			Id = key.Id,
			Name = key.Name,
			Pass = key.Pass
		};
	}
}