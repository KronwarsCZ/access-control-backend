using Core.Entities;
using Infrastructure.Repositories;

namespace Application.IRepositories;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync(DeviceFilters filters, CancellationToken cancellationToken);
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken);
    Task DeleteAsync(IEnumerable<Device> devices, CancellationToken cancellationToken);
}