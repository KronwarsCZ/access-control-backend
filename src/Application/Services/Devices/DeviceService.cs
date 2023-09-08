using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;

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

    public async Task RegisterNewDevice(Device device, CancellationToken cancellationToken)
    {
        
        // poslani mailu a blbosti, registrace v azure iot hubu?
        await _ioTHubDeviceService.RegisterDevice(device.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetAllAsync(cancellationToken);
    }
}
