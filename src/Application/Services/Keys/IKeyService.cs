using Core.Entities;

namespace Application.Services.Keys;

public interface IKeyService
{
    Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken);
}