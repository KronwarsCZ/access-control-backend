using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class KeyService : IKeyService
{
	private readonly IKeyRepository KeyRepository;
    private readonly TimeFreezeService TimeService;
    private readonly IDBContextSaveRepository DbContextSaveRepository;

	public KeyService(IKeyRepository keyRepository, TimeFreezeService timeFreezeService, IDBContextSaveRepository DbContextSaveRepository)
	{
        this.KeyRepository = keyRepository;
        this.TimeService = timeFreezeService;
        this.DbContextSaveRepository = DbContextSaveRepository;
    }

    public Key Create(string name, string pass)
    {
        return new Key()
        {
            Pass = pass,
            CreatedUtc = TimeService.GetUtcTime(),
            UpdatedUtc = TimeService.GetUtcTime(),
            Name = name,
            IsDeleted = false
        };
    }

    public async Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
	{
		return await KeyRepository.GetKeysForDeviceAsync(id, cancellationToken);
	}

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await KeyRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Key>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        return await KeyRepository.AddAsync(keys, cancellationToken);
    }

    public void Delete(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        KeyRepository.Delete(keys, cancellationToken);
    }

    public void Update(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        KeyRepository.Update(keys, cancellationToken);
    }
}