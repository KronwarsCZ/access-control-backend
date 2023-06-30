using Application.IRepositories;
using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Device Create(string owner);
    Task AddAsync(IEnumerable<Device> device, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken);
}