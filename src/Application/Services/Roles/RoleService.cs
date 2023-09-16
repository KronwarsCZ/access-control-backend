using System.Linq.Expressions;
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
    }

    public async Task<IEnumerable<Role>> GetAllAsync(Owner owner,CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetAllAsync(owner, cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken)
    {
        UnitOfWork.RoleRepository.Add(rolesToAdd);
    }
    
    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Role>> FindByConditionAsync(Expression<Func<Role,bool>> predicate, CancellationToken cancellationToken)
    {
        return await UnitOfWork.RoleRepository.FindByConditionAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        UnitOfWork.RoleRepository.Delete(role.ToEnumerable());
    }
}
