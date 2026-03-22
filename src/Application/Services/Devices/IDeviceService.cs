using Application.Services.BasicService;
using Application.Services.Messaging;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Application.Services.Devices;

public interface IDeviceService : IBasicService
{
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task RegisterNewDeviceAsync(Device device, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetAllAsync(Owner owner, CancellationToken ctsToken);

    public Task AttachRoleToDevice(Device device, Role roles, CancellationToken cancellationToken);
    public Task DetachRoleFromDevice(Device device, Role role, CancellationToken cancellationToken);

    Task<IEnumerable<Key>> GetOpeningKeys(Guid deviceId, CancellationToken cancellationToken);
    
    Task DeleteAsync(Device device, CancellationToken cancellationToken);
    Task RenameAsync(Device device, string newName, CancellationToken cancellationToken);
    Task DeviceUpdateFirmwareAsync(FirmwareUpdateMessage commandData, CancellationToken cancellationToken);
}
