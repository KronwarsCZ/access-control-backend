using Application.IRepositories.Devices;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Devices;

public class DeviceRepository : BasicRepository<Device>, IDeviceRepository
{
    public DeviceRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Devices.AsNoTracking().Where(d => d.OwnerId == id).Include(d => d.Roles).ToListAsync(cancellationToken: cancellationToken);
    }    

    public new async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Devices.Where(d => d.Id == id).Include(d => d.Roles).FirstOrDefaultAsync(cancellationToken);
    }

    public new async Task<IEnumerable<Key>> GetOpeningKeys(Guid id, CancellationToken cancellationToken)
    {
        var device =  await Context.Devices
            .AsNoTracking()
            .Where(e => e.Id == id && e.IsDeleted == false)
            .Include(d => d.Roles)
            .ThenInclude(r => r.Keys)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return device.Roles.SelectMany(r => r.Keys).Where(k => k.IsDeleted == false);
    }

    /// <inheritdoc />
    public async Task<Device?> GetByHardwareIdAsync(string hardwareId, CancellationToken cancellationToken)
    {
        return await Context.Devices
            .Where(d => d.HardwareId == hardwareId && !d.IsDeleted)
            .Include(d => d.Roles)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
