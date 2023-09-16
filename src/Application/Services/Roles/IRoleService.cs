using System.Linq.Expressions;
using Application.Services.BasicService;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Application.Services.Roles;

public interface IRoleService : IBasicService
{
    Task<IEnumerable<Role>> GetAllAsync(Owner owner, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Role> newRoleEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(IEnumerable<Role> roles, CancellationToken ctsToken);
    public Task<IEnumerable<Role>> FindByConditionAsync(Expression<Func<Role,bool>> predicate, CancellationToken cancellationToken);
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
