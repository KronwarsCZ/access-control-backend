using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Core.Entities.Devices;

public class Device : OwnedEntity
{
    private Device()
    {
    }

    public string Name { get; set; }
    public DeviceType Type { get; set; }
    public List<Role> Roles { get; } = new();
    
    public static Device Create(Owner owner, string name, DeviceType deviceType)
    {
        var device = CreateEntity<Device>();
        device.OwnerId = owner.Id;
        device.Name = name;
        device.Owner = owner;
        device.Type = deviceType;
        return device;
    }
}
