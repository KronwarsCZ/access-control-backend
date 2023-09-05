using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken);
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task RegisterNewDevice(Device device, CancellationToken cancellationToken);
}
