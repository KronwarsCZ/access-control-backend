using Core.Entities;
using Core.Entities.Keys;
using Framework.Extensions;

namespace Application.Services.Keys;

public class KeyService : IKeyService
{

    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    
	public KeyService(IUnitOfWork.IUnitOfWork context)
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

    public async Task DeleteAsync(Key key, CancellationToken cancellationToken)
    {
        _unitOfWork.KeyRepository.Delete(key.ToEnumerable());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}