using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public Device Create(string owner)
    {
        var device = new Device
        {
            Id = default,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false,
            Owner = null,
            Roles = null,
            Keys = null
        };
        return device;
    }

    public IEnumerable<Guid> Add(IEnumerable<Device> devices)
    {
        return _deviceRepository.Add(devices);
    }

    public async Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken)
    {
        return await _deviceRepository.GetAsync(filters, cancellationToken);
    }
}
