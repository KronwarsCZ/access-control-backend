using Application.IRepositories;
using Core.Entities;
using Infrastructure.Repositories;

namespace Application.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    
    public DeviceService(IDeviceRepository necoRepository)
    {
        _deviceRepository = necoRepository;
    }

    public Device Create(string owner)
    {
        var device = new Device
        {
            Owner = owner
        };
        return device;
    }

    public void Add(IEnumerable<Device> devices)
    {
        _deviceRepository.Add(devices);
    }

    public Task<Device> GetAsync(DeviceFilters filters)
    {
        throw new NotImplementedException();
    }
}