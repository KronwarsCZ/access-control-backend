using Core.Entities.Devices;
using Core.Entities.JoinEntities;
using Core.Entities.Keys;
using Core.Entities.Roles;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IUnitOfWork.IUnitOfWork _unitOfWork;
    private readonly IIoTHubDeviceService _ioTHubDeviceService;
    public DeviceService(IUnitOfWork.IUnitOfWork unitOfWork, IIoTHubDeviceService ioTHubDeviceService)
    {
        this._ioTHubDeviceService = ioTHubDeviceService;
        this._unitOfWork = unitOfWork;
    }
    
    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetByIdAsync(id, cancellationToken);
    }
    
    public async Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetByOwnerIdAsync(id, cancellationToken);
    }

    public async Task RegisterNewDevice(Device device, CancellationToken cancellationToken)
    {
        _unitOfWork.DeviceRepository.Add(device.ToEnumerable());
        await _ioTHubDeviceService.RegisterDeviceToIotHub(device.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AttachRoleToDevice(Device device, Role role, CancellationToken cancellationToken)
    {
        var isConnectionAlreadyInDb = await _unitOfWork.DeviceRoleRepository.IsDeviceRoleConnectionExisting(deviceId: device.Id, roleId: role.Id, cancellationToken);
        Check.Contract(!isConnectionAlreadyInDb, $"Device {device.Id} and Role {role.Id} are already attached to each other.");
        var connection = new DeviceRole { DeviceId = device.Id, RoleId = role.Id };
        _unitOfWork.DeviceRoleRepository.Add(connection.ToEnumerable());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Key>> GetOpeningKeys(Guid deviceId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetOpeningKeys(deviceId, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetAllAsync(cancellationToken);
    }
}
