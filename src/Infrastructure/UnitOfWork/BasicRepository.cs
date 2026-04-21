using System.Linq.Expressions;
using Application.IRepositories;
using Core.Entities.BaseEntities;
using Core.Entities.Owners;
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

    /*public async Task<IEnumerable<T>> GetAllAsync(Owner owner, CancellationToken ctsToken)
    {
        return await Context.Set<T>().Where(e => e.IsDeleted == false && e.GetPrimaryId() == owner.Id).ToListAsync(cancellationToken: ctsToken);
    }*/
    
    public async Task<IEnumerable<T>> GetAllAsync(
        Guid ownerId,
        Expression<Func<T, Guid>> idSelector,
        CancellationToken ctsToken)
    {
        return await Context.Set<T>()
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .Where(BuildIdEqualsPredicate(idSelector, ownerId))
            .ToListAsync(ctsToken);
    }

    private static Expression<Func<T, bool>> BuildIdEqualsPredicate(
        Expression<Func<T, Guid>> selector,
        Guid value)
    {
        var param = selector.Parameters[0];
        var body = Expression.Equal(selector.Body, Expression.Constant(value));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return (await Context.Set<T>().AsNoTracking().Where(e => e.Id == id && e.IsDeleted == false).FirstOrDefaultAsync(cancellationToken: cancellationToken))!;
    }

    public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await Context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
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
