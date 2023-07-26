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
            CreatedUtc = deviceEntity.CreatedUtc,
            UpdatedUtc = deviceEntity.UpdatedUtc,
            IsDeleted = deviceEntity.IsDeleted,
            Owner = null,
            Name = deviceEntity.Name,
            Type = deviceEntity.DeviceType,
        };
    }
    
    internal static DeviceEntity MapDevice(Device device)
    {
        return new DeviceEntity
        {
            Id = device.Id,
            DeviceType = device.Type,
            CreatedUtc = device.CreatedUtc,
            UpdatedUtc = device.UpdatedUtc,
            Name = device.Name,
            IsDeleted = device.IsDeleted
        };
    }
}