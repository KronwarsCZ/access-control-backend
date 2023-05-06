using Application.IRepositories;
using Core;
using Core.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    DeviceRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
    
    private readonly AccessControlDbContext context;
    
    public IEnumerable<Device> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Device> GetAllAsync(DeviceFilters filters)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Device> GetByIdAsync(IEnumerable<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Device> AddAsync(IEnumerable<Device> devices)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Device> DeleteAsync(IEnumerable<Device> devices)
    {
        throw new NotImplementedException();
    }
}