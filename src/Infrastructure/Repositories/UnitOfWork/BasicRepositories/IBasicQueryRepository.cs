using System.Collections.Immutable;
using Core.Entities;
using Infrastructure.DbEntities;

namespace Infrastructure.Repositories.UnitOfWork.BasicRepositories;

public interface IBasicQueryRepository<T, TDb>
    where T : Entity
    where TDb : DbEntity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
