using Application.Services.BasicService;
using Core.Entities.Owners;

namespace Application.Services.Owners;

public interface IOwnerService : IBasicService
{
    Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task RegisterNewOwnerAsync(Owner owner, CancellationToken cancellationToken);
}
