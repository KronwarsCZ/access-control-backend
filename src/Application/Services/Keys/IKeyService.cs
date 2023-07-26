using Core.Entities;

namespace Application.Services;

public interface IKeyService : IBasicService<Key>
{
    Key Create(string name, string pass);
	Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
}