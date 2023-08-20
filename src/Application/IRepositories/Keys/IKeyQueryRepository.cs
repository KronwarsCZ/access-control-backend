using Core.Entities;

namespace Application.IRepositories.Keys;

public interface IKeyQueryRepository
{
    Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken);
}
