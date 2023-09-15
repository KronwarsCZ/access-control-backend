using Application.Services.BasicService;
using Core.Entities.Owners;

namespace Application.Services.Owners;

public interface IOwnerService : IBasicService
{
    Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task RegisterNewOwnerAndAccountAsync(Owner owner, string email, string password, CancellationToken cancellationToken);
}
