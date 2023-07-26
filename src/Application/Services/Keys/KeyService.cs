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
        return await KeyRepository.GetAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        var ids = await KeyRepository.AddAsync(keys, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return ids;
    }

    public async Task Delete(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        KeyRepository.Delete(keys, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    public async Task Update(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        KeyRepository.Update(keys, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }
}