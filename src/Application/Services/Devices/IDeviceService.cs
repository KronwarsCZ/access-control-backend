using Application.IRepositories;
using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Device Create(string owner);
    IEnumerable<Guid> Add(IEnumerable<Device> device);
    Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken);
}