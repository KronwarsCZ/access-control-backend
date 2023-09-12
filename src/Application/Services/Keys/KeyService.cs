using Core.Entities.JoinEntities;
using Core.Entities.Keys;
using Core.Entities.Roles;
using Framework.Extensions;

namespace Application.Services.Keys;

public class KeyService : BasicService.BasicService, IKeyService
{
    public KeyService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Key> GetByIdAsync(Guid keyId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetByIdAsync(keyId, cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken)
    {
        UnitOfWork.KeyRepository.Add(newKeyEnumerable);
    }

    public async Task DeleteAsync(Key key, CancellationToken cancellationToken)
    {
        UnitOfWork.KeyRepository.Delete(key.ToEnumerable());
    }

    public async Task IssueNewKey(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        foreach (var role in roles)
        {
            var isRoleKeyConnectionExisting = await UnitOfWork.RoleKeyRepository.IsRoleKeyConnectionExisting(roleId: role.Id, keyId: key.Id, cancellationToken);
            if (!isRoleKeyConnectionExisting)
            {
                var roleKey = new RoleKey { KeyId = key.Id, RoleId = role.Id};
                UnitOfWork.RoleKeyRepository.Add(roleKey.ToEnumerable());
            }
        }
        UnitOfWork.KeyRepository.Add(key.ToEnumerable());
    }
}