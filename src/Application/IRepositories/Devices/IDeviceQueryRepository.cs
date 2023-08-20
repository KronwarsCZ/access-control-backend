using Core.Entities;

namespace Application.IRepositories.Devices;

public interface IDeviceQueryRepository
{
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken);
}
