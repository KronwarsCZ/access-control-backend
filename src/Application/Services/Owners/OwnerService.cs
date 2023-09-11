using Core.Entities.Owners;
using Framework.Extensions;

namespace Application.Services.Owners;

public class OwnerService : BasicService, IOwnerService
{
    public OwnerService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
    
    public async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.OwnerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task RegisterNewOwnerAsync(Owner owner, CancellationToken cancellationToken)
    {
        UnitOfWork.OwnerRepository.Add(owner.ToEnumerable());
    }
}
