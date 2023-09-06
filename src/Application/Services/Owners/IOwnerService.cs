using Core.Entities;

namespace Application.Services.Owners;

public interface IOwnerService
{
    Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void RegisterNewOwner(Owner owner, CancellationToken cancellationToken);
}
