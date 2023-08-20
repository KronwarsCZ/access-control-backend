using Application.IRepositories.Devices;
using Infrastructure.Database;

namespace Infrastructure.Repositories.Devices;

public class DeviceCommandRepository : IDeviceCommandRepository
{
        
    private readonly AccessControlDbContext context;
    public DeviceCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
}
