using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IKeyService
{
	Task<IEnumerable<Key>> GetAsync(KeyFilters keyFilters, CancellationToken cancellationToken);
}