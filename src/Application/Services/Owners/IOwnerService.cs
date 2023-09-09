using Core.Entities;
using Core.Entities.Owners;

namespace Application.Services.Owners;

public interface IOwnerService
{
    Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task RegisterNewOwnerAsync(Owner owner, CancellationToken cancellationToken);
}
