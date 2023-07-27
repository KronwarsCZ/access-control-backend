using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Framework.Utilities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly AccessControlDbContext _context;
    
    public DeviceRepository(AccessControlDbContext context)
    {
        _context = context;
    }
    
    
    /*public async Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken)
    {
        // var query = _context.Devices.AsQueryable();
        
        // query = filters.Ids.Map(ids => query.Where(d => ids.Contains(d.Id))).GetOrNull();
        
        var dbDevices = await _context.Devices.ToListAsync(cancellationToken);
        return dbDevices.Select(Mapper.MapDevice);
    }

    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res =  await _context.Devices.FindAsync(id, cancellationToken);
        return Mapper.MapDevice(res);
    }

    public IEnumerable<Guid> Add(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.MapDevice).ToList();
        _context.Devices.AddRange(dbDevices);
        return dbDevices.Select(d => d.Id);
    }

    public void Delete(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.MapDevice);
        _context.Devices.RemoveRange(dbDevices);
    }*/

    public async Task<IEnumerable<Device>> GetAsync(CancellationToken cancellationToken)
    {
        var query = await _context.Devices.AsNoTracking().ToArrayAsync(cancellationToken);
        return query.Select(Mapper.MapDevice).ToImmutableArray();
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var dbDevices = devices.Select(Mapper.MapDevice).ToImmutableArray();
        await _context.Devices.AddRangeAsync(dbDevices, cancellationToken);
        return dbDevices.Select(d => d.Id);
    }

    public void Delete(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.MapDevice).ToImmutableArray();
        _context.Devices.RemoveRange(dbDevices);
    }

    public void Update(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.MapDevice).ToImmutableArray();
        _context.Devices.UpdateRange(dbDevices);
    }
}