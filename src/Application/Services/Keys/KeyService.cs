using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.Keys;

public class KeyService : IKeyService
{

    private readonly IUnitOfWork _unitOfWork;
    
	public KeyService(IUnitOfWork context)
	{
        this._unitOfWork = context;

    }

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.KeyRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken)
    {
        _unitOfWork.KeyRepository.Add(newKeyEnumerable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}