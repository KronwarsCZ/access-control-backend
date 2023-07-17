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
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false,
            Owner = null,
            Roles = null,
            Keys = null,
        };
    }
    
    internal static DeviceEntity MapDevice(Device device)
    {
        return new DeviceEntity
        {
            Id = device.Id,
            DeviceType = DeviceType.Lock,
            DbDevicesRoles = null,
            CreatedUtc = default,
            UpdatedUtc = default,
            Name = null,
        };
    }
}