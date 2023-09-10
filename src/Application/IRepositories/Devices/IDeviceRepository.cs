using Core.Entities.Devices;
using Core.Entities.Keys;

namespace Application.IRepositories.Devices;

public interface IDeviceRepository : IBasicRepository<Device>
{
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Key>> GetOpeningKeys(Guid deviceId, CancellationToken cancellationToken);
}
