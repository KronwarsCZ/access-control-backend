using Core.Entities;

namespace Application.IRepositories;

public interface IKeyRepository
{
	Task<IEnumerable<Key>> GetAsync(KeyFilters keyFilters, CancellationToken cancellationToken);
}