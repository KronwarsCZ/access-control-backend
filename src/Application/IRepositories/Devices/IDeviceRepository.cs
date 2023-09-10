using Core.Entities.Devices;

namespace Application.IRepositories.Devices;

public interface IDeviceRepository : IBasicRepository<Device>
{
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
}
