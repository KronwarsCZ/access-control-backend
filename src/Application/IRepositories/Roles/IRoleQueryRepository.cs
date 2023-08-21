using Core.Entities;

namespace Application.IRepositories.Roles;

public interface IRoleQueryRepository
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    void Delete(IEnumerable<Role> roles);
}
