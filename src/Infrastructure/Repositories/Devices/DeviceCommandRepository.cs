using Application.IRepositories.Devices;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories.Devices;

public class DeviceCommandRepository : IDeviceCommandRepository
{
        
    private readonly AccessControlDbContext context;
    public DeviceCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public void Add(Device device)
    {
        context.Devices.Add(Mapper.Map(device));
    }
}
