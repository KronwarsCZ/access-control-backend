using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static Device MapDevice(DeviceEntity deviceEntity)
    {
        return new Device
        {
            Id = deviceEntity.Id,
            Owner = deviceEntity.Owner
        };
    }
    
    internal static DeviceEntity MapDevice(Device Device)
    {
        return new DeviceEntity
        {
            Id = device.Id,
            Owner = device.Owner
        };
    }
}