using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.IRepositories;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    public DeviceRepository(AccessControlDbContext context)
    {
        _context = context;
    }
    
    private readonly AccessControlDbContext _context;
    
    public async Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken)
    {
        var query = _context.Devices.AsQueryable();
        
        //query = filters.Ids.Map(ids => query.Where(d => ids.Contains(d.Id))).GetOrNull();
        
        var dbDevices = await query.ToListAsync(cancellationToken);
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
    }
}