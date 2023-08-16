using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static DeviceType Map(DeviceTypeEntity deviceTypeEntity)
    {
        return deviceTypeEntity switch
        {
            DeviceTypeEntity.Lock => DeviceType.Lock
        };
    }
    
    internal static DeviceTypeEntity Map(DeviceType deviceTypeEntity)
    {
        return deviceTypeEntity switch
        {
            DeviceType.Lock => DeviceTypeEntity.Lock
        };
    }
}
