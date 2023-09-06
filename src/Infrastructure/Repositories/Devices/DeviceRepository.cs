using Application.IRepositories.Devices;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Devices;

public class DeviceRepository : BasicRepository<Device>, IDeviceRepository
{
        
    private readonly AccessControlDbContext _context;
    public DeviceRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }

    public async Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Devices.Where(d => d.OwnerId == id).ToListAsync(cancellationToken: cancellationToken);
    }
}
