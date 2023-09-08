using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task RegisterNewDevice(Owner owner, Device device, CancellationToken cancellationToken);
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken ctsToken);
}
