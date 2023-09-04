using Core.Entities;

namespace Application.IRepositories.Owners;

public interface IOwnerQueryRepository
{
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken);
    Task<Owner> GetByIDAsync(Guid id, CancellationToken cancellationToken);
}
