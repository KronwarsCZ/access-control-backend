using Core.Entities;

namespace Application.Services.Devices;

public interface IDeviceService
{
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken);
    Device Create(Owner owner, string name);
}
