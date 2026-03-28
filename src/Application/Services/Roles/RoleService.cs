using System.Linq.Expressions;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;
using Framework.Extensions;

namespace Application.Services.Roles;

public class RoleService : BasicService.BasicService, IRoleService
{
    public RoleService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task Delete(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        UnitOfWork.RoleRepository.Delete(roles);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(Owner owner,CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetAllAsync(owner.Id, role => role.OwnerId, cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken)
    {
        UnitOfWork.RoleRepository.Add(rolesToAdd);
        await Task.CompletedTask;
    }
    
    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Key>?> GetKeysForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetKeysForRoleAsync(roleId, ownerId, cancellationToken);
    }

    public async Task<IReadOnlyList<Device>?> GetDevicesForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetDevicesForRoleAsync(roleId, ownerId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> FindByConditionAsync(Expression<Func<Role,bool>> predicate, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.FindByConditionAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        UnitOfWork.RoleRepository.Delete(roles);
        await Task.CompletedTask;
    }

    public Task RenameAsync(Role role, string newName, CancellationToken cancellationToken)
    {
        role.Name = newName;
        UnitOfWork.RoleRepository.Update(role.ToEnumerable());
        return Task.CompletedTask;
    }
}
