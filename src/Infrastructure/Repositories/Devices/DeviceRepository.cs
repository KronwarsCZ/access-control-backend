using Application.IRepositories.Devices;
using Core.Entities.Devices;
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
        return await Context.Devices.Where(d => d.OwnerId == id).Include(d => d.Roles).ToListAsync(cancellationToken: cancellationToken);
    }

    public new async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Devices
            .Where(e => e.Id == id && e.IsDeleted == false)
            .Include(d => d.Roles)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}
