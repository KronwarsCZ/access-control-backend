using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mappersdasdasd
{
    internal static Device Map(DeviceEntity deviceEntity)
    {
        return new Device
        {
            Id = deviceEntity.Id,
            CreatedUtc = deviceEntity.CreatedUtc,
            UpdatedUtc = deviceEntity.UpdatedUtc,
            IsDeleted = deviceEntity.IsDeleted,
            Owner = Map(deviceEntity.Owner),
            OwnerId = deviceEntity.OwnerId,
            Name = deviceEntity.Name,
            Type = Map(deviceEntity.Type),
        };
    }
    
    internal static DeviceEntity Map(Device device)
    {
        return new DeviceEntity
        {
            Id = device.Id,
            Type = Map(device.Type),
            CreatedUtc = device.CreatedUtc,
            UpdatedUtc = device.UpdatedUtc,
            Name = device.Name,
            IsDeleted = device.IsDeleted,
            Owner = Map(device.Owner),
            OwnerId = device.OwnerId
        };
    }
}