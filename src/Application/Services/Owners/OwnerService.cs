using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;

namespace Application.Services.Owners;

public class OwnerService : IOwnerService
{
    private readonly IUnitOfWork _unitOfWork;
    public OwnerService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    
    public async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.OwnerRepository.GetByIdAsync(id, cancellationToken);
    }

    public void RegisterNewOwner(Owner owner, CancellationToken cancellationToken)
    {
        _unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
    }
}
