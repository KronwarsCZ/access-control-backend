namespace Application.Services.JointService;

public class DeviceRoleService : BasicService, IDeviceRoleService
{
    public DeviceRoleService(IUnitOfWork.IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public async Task<bool> IsDeviceRoleConnectionExisting(Guid deviceId, Guid roleId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.DeviceRoleRepository.IsDeviceRoleConnectionExisting(deviceId, roleId, cancellationToken);
    }
}
