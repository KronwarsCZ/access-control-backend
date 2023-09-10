using Core.Entities.JoinEntities;

namespace Application.IRepositories.JoinRepositories;

public interface IRoleKeyRepository
{
    public void Add(IEnumerable<RoleKey> deviceRoles);
    public Task<bool> IsRoleKeyConnectionExisting(Guid roleId, Guid keyId, CancellationToken cancellationToken);
}
