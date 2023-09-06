using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;

namespace Application.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IUnitOfWork _unitOfWork;
    public DeviceService(IUnitOfWork context)
    {
        this._unitOfWork = context;
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.DeviceRepository.GetAllAsync(cancellationToken);
    }
}
