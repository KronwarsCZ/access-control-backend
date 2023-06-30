using Application.IRepositories;
using Core.Entities;

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

    public async Task AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        await _deviceRepository.AddAsync(devices, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken)
    {
        return await _deviceRepository.GetAsync(filters, cancellationToken);
    }
}