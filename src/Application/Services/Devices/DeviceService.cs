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
    
    public Device Create(Guid id, string name, bool isDeleted, DeviceType type)
    {
        var device = new Device
        {
            Id = id,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            IsDeleted = isDeleted,
            Owner = null,
            Name = name,
            Type = type
        };
        return device;
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _deviceRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> AddAsync(IEnumerable<Device> things, CancellationToken cancellationToken)
    {
        return await _deviceRepository.AddAsync(things, cancellationToken);
    }

    public async Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _deviceRepository.DeleteAsync(ids, cancellationToken);
    }

    public async Task<Device> UpdateAsync(Guid id, Device thingToUpdate, CancellationToken cancellationToken)
    {
        return await _deviceRepository.UpdateAsync(id, thingToUpdate, cancellationToken);
    }

    
}
