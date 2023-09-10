using Core.Entities.JoinEntities;
using Core.Entities.Keys;
using Core.Entities.Roles;
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

    public async Task IssueNewKey(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        foreach (var role in roles)
        {
            var isRoleKeyConnectionExisting = await _unitOfWork.RoleKeyRepository.IsRoleKeyConnectionExisting(roleId: role.Id, keyId: key.Id, cancellationToken);
            if (!isRoleKeyConnectionExisting)
            {
                var roleKey = new RoleKey { KeyId = key.Id, RoleId = role.Id};
                _unitOfWork.RoleKeyRepository.Add(roleKey.ToEnumerable());
            }
        }
        _unitOfWork.KeyRepository.Add(key.ToEnumerable());
        await _unitOfWork.SaveChangesAsync(cancellationToken);    
    }
}