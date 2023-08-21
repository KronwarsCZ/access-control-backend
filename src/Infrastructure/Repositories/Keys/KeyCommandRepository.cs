using Application.IRepositories.Keys;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories.Keys;

public class KeyCommandRepository : IKeyCommandRepository
{
        
    private readonly AccessControlDbContext context;
    public KeyCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public void Update(IEnumerable<Key> keys)
    {
        context.Keys.UpdateRange(keys.Select(Mapper.Map));
    }

    public void Delete(IEnumerable<Key> keys)
    {
        context.Keys.RemoveRange(keys.Select(Mapper.Map));
    }

    public IEnumerable<Guid> Add(IEnumerable<Key> keys)
    {
        var dbKeys = keys.Select(Mapper.Map);
        context.Keys.AddRange(dbKeys);
        return dbKeys.Select(k => k.Id);
    }
}
