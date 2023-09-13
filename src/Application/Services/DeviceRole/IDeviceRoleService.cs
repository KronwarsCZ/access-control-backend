namespace Application.Services.DeviceRole;

public interface IDeviceRoleService
{
    Task<bool> IsDeviceRoleConnectionExisting(Guid deviceId, Guid roleId, CancellationToken cancellationToken);
}
