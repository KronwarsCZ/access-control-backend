using Core.Entities;

namespace Application.IRepositories.Devices;

public interface IDeviceQueryRepository
{
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken);

    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
}
