using Core.Entities;
using Infrastructure.Repositories;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Device Create(string owner);
    Task<Device> GetAsync(DeviceFilters filters);
}