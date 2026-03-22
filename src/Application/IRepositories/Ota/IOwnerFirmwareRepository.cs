using Core.Entities.Ota;

namespace Application.IRepositories.Ota;

public interface IOwnerFirmwareRepository : IBasicRepository<OwnerFirmware>
{
    Task<OwnerFirmware?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
    Task<IEnumerable<OwnerFirmware>> GetAllWithOwnerAsync(CancellationToken cancellationToken);
}
