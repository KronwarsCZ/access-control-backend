using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIoTHubDeviceService _ioTHubDeviceService;
    public DeviceService(IUnitOfWork unitOfWork, IIoTHubDeviceService ioTHubDeviceService)
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

    public async Task RegisterNewDevice(Owner owner, Device device, CancellationToken cancellationToken)
    {
        owner.Devices.Add(device);

        await _ioTHubDeviceService.RegisterDevice(device.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRolesToDevice(Device device, List<Role> roles, CancellationToken cancellationToken)
    {
        device.Roles.AddRange(roles);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetAllAsync(cancellationToken);
    }
}
