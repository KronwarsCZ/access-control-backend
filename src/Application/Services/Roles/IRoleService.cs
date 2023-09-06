using Core.Entities;

namespace Application.Services.Roles;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken ctsToken);
    Task AddAsync(IEnumerable<Role> newRoleEnumerable, CancellationToken ctsToken);
}
