using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IDeviceService : IBasicService<Device>
{
    Device Create(string name, DeviceType type, Core.Entities.Owner owner);
}
