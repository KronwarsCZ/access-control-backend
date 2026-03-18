using Application.Services.Messaging;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Devices;

public class DeviceService : BasicService.BasicService, IDeviceService
{
    private readonly IIoTHubDeviceService _ioTHubDeviceService;
    public DeviceService(IUnitOfWork.IUnitOfWork unitOfWork, IIoTHubDeviceService ioTHubDeviceService)
        : base(unitOfWork)
    {
        _ioTHubDeviceService = ioTHubDeviceService;
    }

    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.DeviceRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.DeviceRepository.GetByOwnerIdAsync(id, cancellationToken);
    }

    public async Task RegisterNewDeviceAsync(Device device, CancellationToken cancellationToken)
    {
        UnitOfWork.DeviceRepository.Add(device.ToEnumerable());
        await _ioTHubDeviceService.RegisterDeviceToIotHub(device.Id);
    }

    public async Task AttachRoleToDevice(Device device, Role role, CancellationToken cancellationToken)
    {
        var isConnectionAlreadyInDb = await UnitOfWork.DeviceRoleRepository.IsDeviceRoleConnectionExisting(deviceId: device.Id, roleId: role.Id, cancellationToken);
        Check.Contract(!isConnectionAlreadyInDb, $"Device {device.Id} and Role {role.Id} are already attached to each other.");
        var connection = new Core.Entities.JoinEntities.DeviceRole { DeviceId = device.Id, RoleId = role.Id };
        UnitOfWork.DeviceRoleRepository.Add(connection.ToEnumerable());
    }

    public async Task DetachRoleFromDevice(Device device, Role role, CancellationToken cancellationToken)
    {
        await UnitOfWork.DeviceRoleRepository.RemoveAsync(deviceId: device.Id, roleId: role.Id, cancellationToken);
    }

    public async Task<IEnumerable<Key>> GetOpeningKeys(Guid deviceId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.DeviceRepository.GetOpeningKeys(deviceId, cancellationToken);
    }

    public async Task DeleteAsync(Device device, CancellationToken cancellationToken)
    {
        UnitOfWork.DeviceRepository.Delete(device.ToEnumerable());
    }

    public Task RenameAsync(Device device, string newName, CancellationToken cancellationToken)
    {
        device.Name = newName;
        UnitOfWork.DeviceRepository.Update(device.ToEnumerable());
        return Task.CompletedTask;
    }

    public async Task DeviceUpdateFirmwareAsync(FirmwareUpdateMessage commandData, CancellationToken cancellationToken)
    {
        var device = await UnitOfWork.DeviceRepository.GetByIdAsync(commandData.DeviceId, cancellationToken);
        // todo - jakym zpusobem tam dopravime tu novou binarku
    }

    public async Task<IEnumerable<Device>> GetAllAsync(Owner owner, CancellationToken cancellationToken)
    {
        return await UnitOfWork.DeviceRepository.GetAllAsync(owner.Id, o => o.OwnerId, cancellationToken);
    }
}
