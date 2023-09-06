using Core.Entities;

namespace Application.IRepositories.Roles;

public interface IRoleQueryRepository
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Delete(IEnumerable<Role> roles);
}
