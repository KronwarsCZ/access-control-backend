using Application.IRepositories.Devices;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Devices;

public class DeviceQueryRepository : IDeviceQueryRepository
{
        
    private readonly AccessControlDbContext context;
    public DeviceQueryRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbDevices = await context.Devices.ToListAsync(cancellationToken);
        return dbDevices.Select(Mapper.Map);
    }
    
    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbDevice = await context.Devices.FindAsync(new object[] {id}, cancellationToken);
        return Mapper.Map(dbDevice);
    }

    public async Task<IEnumerable<Device>> GetByOwnerIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbDevices = await context.Devices.Where(device => device.OwnerId == id).ToListAsync(cancellationToken);
        return dbDevices.Select(Mapper.Map);
    }
}
