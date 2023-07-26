using Core.Entities;

namespace Application.IRepositories;

public interface IKeyRepository : IBasicRepository<Key>
{
	Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
}