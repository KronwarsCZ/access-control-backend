using Core.Entities.JoinEntities;

namespace Application.IRepositories.JoinRepositories;

public interface IDeviceRoleRepository
{
    public void Add(IEnumerable<DeviceRole> deviceRoles);
    public Task<bool> IsDeviceRoleConnectionExisting(Guid deviceId, Guid roleId, CancellationToken cancellationToken);
    public Task RemoveAsync(Guid deviceId, Guid roleId, CancellationToken cancellationToken);
}
