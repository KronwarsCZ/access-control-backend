using Core.Entities;

namespace Application.Services.Roles;

public interface IRoleService
{
    Task Delete(IEnumerable<Role> roles, CancellationToken cancellationToken);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken);
    Role Create(string createRoleText);
}
