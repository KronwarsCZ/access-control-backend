using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.IRepositories.Roles;

public interface IRoleRepository : IBasicRepository<Role>
{
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
