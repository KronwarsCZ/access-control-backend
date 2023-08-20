using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.Keys;

public class KeyService : IKeyService
{

    private readonly IUnitOfWork context;
    
	public KeyService(IUnitOfWork context)
	{
        this.context = context;

    }
    public async Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.KeyQueryRepository.GetKeysForDeviceAsync(id, cancellationToken);
	}

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.KeyQueryRepository.GetAllAsync(cancellationToken);
    }
    
    public async Task Delete(IEnumerable<Key> keysToDelete, CancellationToken cancellationToken)
    {
        context.KeyCommandRepository.Delete(keysToDelete);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Key Create(string createKeyText, string s)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Key> keysToAdd, CancellationToken cancellationToken)
    {
        var ids = context.KeyCommandRepository.Add(keysToAdd);
        await context.SaveChangesAsync(cancellationToken);
        return ids;
    }

    public async Task DeleteAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        context.KeyCommandRepository.Delete(keys);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(IEnumerable<Key> keysToUpdate, CancellationToken cancellationToken)
    {
        context.KeyCommandRepository.Update(keysToUpdate);
        await context.SaveChangesAsync(cancellationToken);
    }
}