using Application.IRepositories;
using Core;
using Core.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    public DeviceRepository(AccessControlDbContext context)
    {
        _context = context;
    }
    
    private readonly AccessControlDbContext _context;
    
    public Task<IEnumerable<Device>> GetAllAsync(DeviceFilters filters, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Devices.FindAsync(id, cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        await _context.Devices.AddRangeAsync(devices, cancellationToken);
    }

    public Task DeleteAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}