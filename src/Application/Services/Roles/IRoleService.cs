using Core.Entities;

namespace Application.Services.Roles;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Role> newRoleEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(Role role, CancellationToken ctsToken);
}
