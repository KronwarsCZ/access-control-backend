using System.Collections.Immutable;
using System.Data.SqlTypes;
using Application.IRepositories;
using Core.Entities;
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
        return dbDevices.Select(Mapper.Map);
    }

    public async Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res =  await _context.Devices.FindAsync(id, cancellationToken);
        return Mapper.Map(res);
    }

    public IEnumerable<Guid> Add(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.Map).ToList();
        _context.Devices.AddRange(dbDevices);
        return dbDevices.Select(d => d.Id);
    }

    public void Delete(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.Map);
        _context.Devices.RemoveRange(dbDevices);
    }*/

    public async Task<IEnumerable<Device>> GetAsync(CancellationToken cancellationToken)
    {
        var query = await _context.Devices.AsNoTracking().ToArrayAsync(cancellationToken);
        return query.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<Device> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = await _context.Devices.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (query is null)
        {
            throw new SqlNullValueException("The given primary key doesnt exist in database");
        }

        return Mapper.Map(query);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var dbDevices = devices.Select(Mapper.Map).ToImmutableArray();
        await _context.Devices.AddRangeAsync(dbDevices, cancellationToken);
        return dbDevices.Select(d => d.Id);
    }

    public void Delete(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.Map).ToImmutableArray();
        _context.Devices.RemoveRange(dbDevices);
    }

    public void Update(IEnumerable<Device> devices)
    {
        var dbDevices = devices.Select(Mapper.Map).ToImmutableArray();
        _context.Devices.UpdateRange(dbDevices);
    }
}