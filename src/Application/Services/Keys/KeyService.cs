using Core.Entities.Devices;
using Core.Entities.JoinEntities;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;
using Framework.Extensions;

namespace Application.Services.Keys;

public class KeyService : BasicService.BasicService, IKeyService
{
    public KeyService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<IEnumerable<Key>> GetAllAsync(Owner owner, CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetAllAsync(owner.Id, key => key.OwnerId, cancellationToken);
    }

    public async Task<Key> GetByIdAsync(Guid keyId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetByIdAsync(keyId, cancellationToken);
    }

    public async Task<Key> GetByIdWithRolesAsync(Guid keyId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetByIdWithRolesAsync(keyId, cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken)
    {
        UnitOfWork.KeyRepository.Add(newKeyEnumerable);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Key key, CancellationToken cancellationToken)
    {
        UnitOfWork.KeyRepository.Delete(key.ToEnumerable());
        await Task.CompletedTask;
    }

    public async Task IssueNewKey(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        foreach (var role in roles)
        {
            var isRoleKeyConnectionExisting = await UnitOfWork.RoleKeyRepository.IsRoleKeyConnectionExisting(roleId: role.Id, keyId: key.Id, cancellationToken);
            
            if (isRoleKeyConnectionExisting) continue;
            
            var roleKey = new RoleKey { KeyId = key.Id, RoleId = role.Id};
            UnitOfWork.RoleKeyRepository.Add(roleKey.ToEnumerable());
        }
        UnitOfWork.KeyRepository.Add(key.ToEnumerable());
    }
    
    public async Task<IEnumerable<Device>> GetDevicesToOpen(Key key, CancellationToken cancellationToken)
    {
        return await UnitOfWork.KeyRepository.GetDevicesToOpen(key.Id, cancellationToken);
    }

    public async Task AttachRoleToKey(Key key, Role role, CancellationToken cancellationToken)
    {
        // Key and Role must! be in db already
        var isRoleKeyConnectionExisting = await UnitOfWork.RoleKeyRepository.IsRoleKeyConnectionExisting(roleId: role.Id, keyId: key.Id, cancellationToken);
        if (isRoleKeyConnectionExisting) return;
        
        var roleKey = new RoleKey { KeyId = key.Id, RoleId = role.Id};
        
        UnitOfWork.RoleKeyRepository.Add(roleKey.ToEnumerable());
    }

    public async Task DetachRoleFromKey(Key key, Role role, CancellationToken cancellationToken)
    {
        await UnitOfWork.RoleKeyRepository.RemoveAsync(roleId: role.Id, keyId: key.Id, cancellationToken);
    }

    public Task RenameAsync(Key key, string newName, CancellationToken cancellationToken)
    {
        key.Name = newName;
        UnitOfWork.KeyRepository.Update(key.ToEnumerable());
        return Task.CompletedTask;
    }
}