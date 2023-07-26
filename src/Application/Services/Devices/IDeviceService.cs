using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IDeviceService : IBasicService<Device>
{
    Device Create(Guid id, string name, bool isDeleted, DeviceType type);
}
