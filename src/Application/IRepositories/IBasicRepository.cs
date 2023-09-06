using System.Linq.Expressions;
using Core.Entities;

namespace Application.UnitOfWorkStuff_to_sort;

public interface IBasicRepository<T>
    where T: Entity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ctsToken);

    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<T> FindByConditionAsync(Expression<Func<T, bool>> predicate);

    void Add(IEnumerable<T> entitiesToAdd);

    void Update(IEnumerable<T> entitiesToUpdate);

    void Delete(IEnumerable<T> entitiesToDelete);

    void HardDelete(IEnumerable<T> entitiesToDelete);
}
