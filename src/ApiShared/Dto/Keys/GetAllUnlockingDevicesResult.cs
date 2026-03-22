using ApiShared.Dto.Devices;

namespace ApiShared.Dto.Keys;

public class GetAllUnlockingDevicesResult
{
    public IEnumerable<DeviceResult> Devices { get; init; } = null!;
}
