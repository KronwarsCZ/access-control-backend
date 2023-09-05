using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.Owners;

public class OwnerService : IOwnerService
{
    private readonly IUnitOfWork context;
    public OwnerService(IUnitOfWork context)
    {
        this.context = context;
    }
    
    public async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.OwnerQueryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.OwnerQueryRepository.GetAllAsync(cancellationToken);
    }

    public void RegisterNewOwner(Owner owner, CancellationToken cancellationToken)
    {
        context.OwnerCommandRepository.Add(owner);
        context.SaveChangesAsync(cancellationToken);
    }
}
