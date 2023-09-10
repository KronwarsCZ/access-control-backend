using System.Linq.Expressions;
using Application.IRepositories;
using Core.Entities.BaseEntities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork;

public class BasicRepository<T> : IBasicRepository<T>
    where T : Entity
{
    protected readonly AccessControlDbContext Context;

    public BasicRepository(AccessControlDbContext context)
    {
        Context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ctsToken)
    {
        return await Context.Set<T>().Where(e => e.IsDeleted == false).ToListAsync(cancellationToken: ctsToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Set<T>().Where(e => e.Id == id && e.IsDeleted == false).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
    }

    public void Add(IEnumerable<T> entitiesToAdd)
    {
        Context.Add(entitiesToAdd);
    }

    public void Update(IEnumerable<T> entitiesToUpdate)
    {
        var timeStamp = DateTime.UtcNow;
        foreach (var entity in entitiesToUpdate)
        {
            entity.UpdatedUtc = timeStamp;
        }
        Context.Set<T>().UpdateRange(entitiesToUpdate);
    }

    public void Delete(IEnumerable<T> entitiesToDelete)
    {
        var timeStamp = DateTime.UtcNow;

        foreach (var entityToDelete in entitiesToDelete)
        {
            entityToDelete.IsDeleted = true;
            entityToDelete.DeletedUtc = timeStamp;
        }
        Context.Set<T>().UpdateRange(entitiesToDelete);
    }
    
    public void HardDelete(IEnumerable<T> entitiesToDelete)
    {
        Context.Set<T>().RemoveRange(entitiesToDelete);
    }
}
