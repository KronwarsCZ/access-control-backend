using Core.Entities;

namespace Application.IRepositories.Devices;

public interface IDeviceCommandRepository
{
    void Add(Device device);
}
