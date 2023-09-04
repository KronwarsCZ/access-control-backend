using Core.Entities;

namespace Application.Services.Owners;

public interface IOwnerService
{
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<Owner> GetByIDAsync(Guid id, CancellationToken cancellationToken);

}
