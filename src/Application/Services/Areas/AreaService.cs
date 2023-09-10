using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;

namespace Application.Services.Areas;

public class AreaService : IAreaService
{
    private readonly IUnitOfWork _unitOfWork;
    public AreaService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    
    public async Task<Area> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.AreaRepository.GetByIdAsync(id, cancellationToken);
    }
}
