namespace ApiShared.Dto.Devices;

public class GetAllDevicesResult
{
    public IEnumerable<DeviceResult> Devices { get; set; } = null!;
}

