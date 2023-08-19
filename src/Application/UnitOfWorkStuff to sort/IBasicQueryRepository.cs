using Core.Entities;

namespace Application.UnitOfWorkStuff_to_sort;

public interface IBasicQueryRepository<T>
    where T : Entity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
