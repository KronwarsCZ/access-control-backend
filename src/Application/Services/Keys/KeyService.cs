using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class KeyService : IKeyService
{
	private readonly IKeyRepository _keyRepository;

	public KeyService(IKeyRepository keyRepository)
	{
		_keyRepository = keyRepository;
	}

	public async Task<IEnumerable<Key>> GetAsync(KeyFilters keyFilters, CancellationToken cancellationToken)
	{
		return await _keyRepository.GetAsync(keyFilters, cancellationToken);
	}
}