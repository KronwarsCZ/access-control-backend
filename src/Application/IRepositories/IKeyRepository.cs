using Core.Entities;

namespace Application.IRepositories;

public interface IKeyRepository
{
	Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Key>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken);
    Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<Key> UpdateAsync(Guid keyId, Key key, CancellationToken cancellationToken);
}