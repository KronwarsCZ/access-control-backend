#nullable enable
using Core.Entities.BaseEntities;
using Core.Entities.Devices;
using Core.Entities.Owners;

namespace Core.Entities.DeviceIoTInfo;

public class DeviceIoTInfo : OwnedEntity
{
    private DeviceIoTInfo() {}
    public Guid DeviceId { get; set; }
    public Device Device { get; set; }
    
    public string IoTHubDeviceId { get; set; } = null!;

    public ConnectionState ConnectionState { get; set; }

    public string? FirmwareVersion { get; set; }
    
    
    public static DeviceIoTInfo Create(Owner owner, Device device, string ioTHubDeviceId, ConnectionState cs, string? fwVersion)
    {
        var iotdeviceinfo = CreateEntity<DeviceIoTInfo>();
        iotdeviceinfo.Owner = owner;
        iotdeviceinfo.OwnerId = owner.Id;
        iotdeviceinfo.Device = device;
        iotdeviceinfo.DeviceId = device.Id;
        
        iotdeviceinfo.IoTHubDeviceId = ioTHubDeviceId;
        iotdeviceinfo.ConnectionState = cs;
        iotdeviceinfo.FirmwareVersion = fwVersion;
        return iotdeviceinfo;
    }
}
