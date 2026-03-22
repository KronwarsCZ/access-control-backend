#nullable enable
using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Core.Entities.Devices;

public class Device : OwnedEntity
{
    private Device() { }

    /// <summary>
    /// User-friendly display name (can be changed via UI).
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Hardware device identifier (e.g., DEV-AABBCCDDEEFF).
    /// This is the immutable identifier from the physical device.
    /// Null for manually created devices.
    /// </summary>
    public string? HardwareId { get; set; }
    
    public DeviceType Type { get; set; }
    public List<Role> Roles { get; } = [];
    
    public DeviceIoTInfo.DeviceIoTInfo? IoTInfo { get; set; }
    
    public static Device Create(Owner owner, string name, DeviceType deviceType)
    {
        var device = CreateEntity<Device>();
        device.OwnerId = owner.Id;
        device.Name = name;
        device.Owner = owner;
        device.Type = deviceType;
        return device;
    }

    /// <summary>
    /// Creates a device from hardware provisioning with a default display name.
    /// </summary>
    public static Device CreateFromProvisioning(Owner owner, string hardwareId, DeviceType deviceType)
    {
        var device = CreateEntity<Device>();
        device.OwnerId = owner.Id;
        device.Owner = owner;
        device.HardwareId = hardwareId;
        device.Name = $"New Lock ({hardwareId[^6..]})"; // e.g., "New Lock (DDEEFF)"
        device.Type = deviceType;
        return device;
    }
}
