using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static Device MapDevice(DbDevice dbDevice)
    {
        return new Device
        {
            Id = dbDevice.Id,
            Owner = dbDevice.Owner
        };
    }
    
    internal static DbDevice MapDevice(Device device)
    {
        return new DbDevice
        {
            Id = device.Id,
            Owner = device.Owner
        };
    }
}