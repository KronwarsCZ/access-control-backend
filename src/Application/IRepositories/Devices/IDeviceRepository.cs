using Core.Entities.Devices;
using Core.Entities.Keys;

namespace Application.IRepositories.Devices;

public interface IDeviceRepository : IBasicRepository<Device>
{
    Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Key>> GetOpeningKeys(Guid deviceId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets a device by its hardware ID (e.g., DEV-AABBCCDDEEFF).
    /// </summary>
    Task<Device?> GetByHardwareIdAsync(string hardwareId, CancellationToken cancellationToken);
}
