using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Roles;

namespace Application.IRepositories.Roles;

public interface IRoleRepository : IBasicRepository<Role>
{
    /// <summary>
    /// Returns keys linked to the role when the role exists, is not deleted, and belongs to <paramref name="ownerId"/>.
    /// Returns null if the role does not exist or is not owned by <paramref name="ownerId"/>.
    /// </summary>
    Task<IReadOnlyList<Key>?> GetKeysForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns devices linked to the role when the role exists, is not deleted, and belongs to <paramref name="ownerId"/>.
    /// Returns null if the role does not exist or is not owned by <paramref name="ownerId"/>.
    /// </summary>
    Task<IReadOnlyList<Device>?> GetDevicesForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken);
}
