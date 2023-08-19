using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService : IBasicService<Device>
{
    Device Create(string name, DeviceType type, Core.Entities.Owner owner);
}
