using Core.Entities;
using Core.Entities.Owners;
using Framework.Extensions;

namespace Application.Services.Owners;

public class OwnerService : IOwnerService
{
    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    public OwnerService(IUnitOfWork.IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    
    public async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.OwnerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task RegisterNewOwnerAsync(Owner owner, CancellationToken cancellationToken)
    {
        _unitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
