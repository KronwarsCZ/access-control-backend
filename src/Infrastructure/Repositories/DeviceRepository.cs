using Application.IRepositories;
using Core;
using Core.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    public DeviceRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
    
    private readonly AccessControlDbContext context;
    
    public IEnumerable<Device> GetAllAsync(DeviceFilters filters)
    {
        throw new NotImplementedException();
    }

    public Device GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<Device> devices)
    {
        context.Devices.AddRange(devices);
        
    }

    public void DeleteAsync(IEnumerable<Device> devices)
    {
        throw new NotImplementedException();
    }
}