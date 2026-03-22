using Application.IRepositories.DeviceProvisioning;
using Core.Entities.DeviceProvisioning;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.DeviceProvisioning;

public class ProvisioningSecretRepository : BasicRepository<ProvisioningSecret>, IProvisioningSecretRepository
{
    public ProvisioningSecretRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<ProvisioningSecret?> GetBySecretHashAsync(string hash, CancellationToken cancellationToken)
    {
        return await Context.ProvisioningSecrets
            .Where(s => s.SecretHash == hash && s.IsActive && !s.IsDeleted)
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProvisioningSecret>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        return await Context.ProvisioningSecrets
            .Where(s => s.IsActive && !s.IsDeleted)
            .Include(s => s.Owner)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProvisioningSecret?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return await Context.ProvisioningSecrets
            .Where(s => s.OwnerId == ownerId && !s.IsDeleted)
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
