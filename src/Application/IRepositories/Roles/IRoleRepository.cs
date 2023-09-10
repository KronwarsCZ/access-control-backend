using Core.Entities.Roles;

namespace Application.IRepositories.Roles;

public interface IRoleRepository : IBasicRepository<Role>
{
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
