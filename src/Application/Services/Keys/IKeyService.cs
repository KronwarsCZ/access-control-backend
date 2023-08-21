using Core.Entities;

namespace Application.Services.Keys;

public interface IKeyService
{ 
    Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken);
    Task Delete(IEnumerable<Key> keysToDelete, CancellationToken cancellationToken);
    Task Update(IEnumerable<Key> keysToUpdate, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> AddAsync(IEnumerable<Key> keysToAdd, CancellationToken cancellationToken);
}