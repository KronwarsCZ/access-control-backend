using Core.Entities;

namespace Application.IRepositories.Roles;

public interface IRoleCommandRepository
{
    void Add(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken);
}
