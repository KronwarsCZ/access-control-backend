using Application.IRepositories.DeviceProvisioning;
using Core.Entities.DeviceProvisioning;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.DeviceProvisioning;

/// <summary>
/// Repository implementation for provisioned device operations.
/// </summary>
public class ProvisionedDeviceRepository : BasicRepository<ProvisionedDevice>, IProvisionedDeviceRepository
{
    public ProvisionedDeviceRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ProvisionedDevice?> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken)
    {
        return await Context.ProvisionedDevices
            .Where(d => d.DeviceId == deviceId && !d.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProvisionedDevice?> GetByCertificateThumbprintAsync(string thumbprint, CancellationToken cancellationToken)
    {
        return await Context.ProvisionedDevices
            .Where(d => d.CertificateThumbprint == thumbprint && !d.IsDeleted && d.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProvisionedDevice>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        return await Context.ProvisionedDevices
            .Where(d => d.IsActive && !d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetActiveCountByOwnerAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return await Context.ProvisionedDevices
            .Where(d => d.IsActive && !d.IsDeleted && d.OwnerId == ownerId)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProvisionedDevice>> GetExpiringCertificatesAsync(int days, CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        
        return await Context.ProvisionedDevices
            .Where(d => d.IsActive && !d.IsDeleted && d.CertificateExpiresUtc <= cutoffDate)
            .OrderBy(d => d.CertificateExpiresUtc)
            .ToListAsync(cancellationToken);
    }
}
