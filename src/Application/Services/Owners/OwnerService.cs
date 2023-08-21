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

    public async Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.OwnerQueryRepository.GetAllAsync(cancellationToken);
    }
}
