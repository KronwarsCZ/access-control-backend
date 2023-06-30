using Core.Entities;
using Infrastructure.Repositories;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Device Create(string owner);
    Task AddAsync(IEnumerable<Device> device, CancellationToken cancellationToken);
    Task<Device> GetAsync(DeviceFilters filters, CancellationToken cancellationToken);
}