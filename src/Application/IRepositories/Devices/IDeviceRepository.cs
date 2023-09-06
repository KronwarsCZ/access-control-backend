using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.IRepositories.Devices;

public interface IDeviceRepository : IBasicRepository<Device>
{
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
}
