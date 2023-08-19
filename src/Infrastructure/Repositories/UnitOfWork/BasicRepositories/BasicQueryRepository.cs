using System.Collections.Immutable;
using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.UnitOfWork.BasicRepositories;

public class BasicQueryRepository<T, TDb> : IBasicQueryRepository<T, TDb>
    where T : Entity
    where TDb : MappableDbEntity<T>
{
    public readonly AccessControlDbContext Context;
    
    public BasicQueryRepository(AccessControlDbContext context)
    {
        Context = context;
    }
    
    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity =  await Context.Set<TDb>().FindAsync(id);
        return entity.MapRef(e => e.Map());
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities =  await Context.Set<TDb>().Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
        return entities.Select(e => e.Map());
    }
}
