using System.Linq.Expressions;
using Application.Services.BasicService;
using Core.Entities.Roles;

namespace Application.Services.Roles;

public interface IRoleService : IBasicService
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Role> newRoleEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(Role role, CancellationToken ctsToken);
    public Task<IEnumerable<Role>> FindByConditionAsync(Expression<Func<Role,bool>> predicate, CancellationToken cancellationToken);
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
