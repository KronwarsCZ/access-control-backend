using Core.Entities.DeviceProvisioning;

namespace Application.IRepositories.DeviceProvisioning;

public interface IProvisioningSecretRepository : IBasicRepository<ProvisioningSecret>
{
    Task<ProvisioningSecret?> GetBySecretHashAsync(string hash, CancellationToken cancellationToken);
    Task<IEnumerable<ProvisioningSecret>> GetAllActiveAsync(CancellationToken cancellationToken);
    Task<ProvisioningSecret?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
}
