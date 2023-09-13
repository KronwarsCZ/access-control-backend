namespace Application.Services.JointService;

public interface IDeviceRoleService
{
    Task<bool> IsDeviceRoleConnectionExisting(Guid deviceId, Guid roleId, CancellationToken cancellationToken);
}
