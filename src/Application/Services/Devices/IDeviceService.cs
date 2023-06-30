using Core.Entities;
using Infrastructure.Repositories;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Device Create(string owner);
    void Add(IEnumerable<Device> device);
    Task<Device> GetAsync(DeviceFilters filters);
}