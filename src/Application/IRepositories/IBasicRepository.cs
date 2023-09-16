using System.Linq.Expressions;
using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Application.IRepositories;

public interface IBasicRepository<T>
    where T: Entity
{
    Task<IEnumerable<T>> GetAllAsync(Owner owner,CancellationToken ctsToken);

    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    void Add(IEnumerable<T> entitiesToAdd);

    void Update(IEnumerable<T> entitiesToUpdate);

    void Delete(IEnumerable<T> entitiesToDelete);

    void HardDelete(IEnumerable<T> entitiesToDelete);
}
