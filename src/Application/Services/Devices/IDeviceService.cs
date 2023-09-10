using Core.Entities.Devices;
using Core.Entities.Roles;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task RegisterNewDevice(Device device, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken ctsToken);

    public Task AttachRoleToDevice(Device device, Role roles, CancellationToken cancellationToken);

}
