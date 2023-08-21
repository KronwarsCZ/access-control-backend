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
}
