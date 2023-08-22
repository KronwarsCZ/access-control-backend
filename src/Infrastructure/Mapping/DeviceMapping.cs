using Core.Entities;
using Framework.Extensions;
using Infrastructure.DbEntities;

namespace Infrastructure.Mapping;

internal static partial class Mapper
{
    internal static Device Map(DeviceEntity deviceEntity)
    {
        return new Device
        {
            Id = deviceEntity.Id,
            CreatedUtc = deviceEntity.CreatedUtc,
            UpdatedUtc = deviceEntity.UpdatedUtc,
            IsDeleted = deviceEntity.IsDeleted,
            Owner = deviceEntity.Owner.MapRef(Map),
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
            Owner = device.Owner.MapRef(Map),
            OwnerId = device.OwnerId
        };
    }
}