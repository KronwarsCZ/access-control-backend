using Core.Entities;

namespace Application.Services;

public interface IKeyService : IBasicService<Key>
{
    Key Create(Guid id, string name, bool IsDeleted, string pass);
	Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
    

}