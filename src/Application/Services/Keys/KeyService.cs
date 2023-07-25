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

    public Key Create(Guid id, string name, bool isDeleted, string pass)
    {
        return new Key()
        {
            Id = id,
            Pass = pass,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            Name = name,
            IsDeleted = isDeleted
        };
    }

    public async Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
	{
		return await _keyRepository.GetKeysForDeviceAsync(id, cancellationToken);
	}

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _keyRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Key>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        return await _keyRepository.AddAsync(keys, cancellationToken);
    }

    public async Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _keyRepository.DeleteAsync(ids, cancellationToken);
    }

    public async Task<Key> UpdateAsync(Guid keyId, Key key, CancellationToken cancellationToken)
    {
        return await _keyRepository.UpdateAsync(keyId, key, cancellationToken);
    }
}