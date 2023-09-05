using Core.Entities;

namespace Application.Services.Owners;

public interface IOwnerService
{
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void RegisterNewOwner(Owner owner, CancellationToken cancellationToken);
}
