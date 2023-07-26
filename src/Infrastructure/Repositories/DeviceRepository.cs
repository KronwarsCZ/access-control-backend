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

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var query = await _context.Devices.ToArrayAsync(cancellationToken);
            return query.Select(Mapper.MapDevice).ToImmutableArray();
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Device>> AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var device in devices)
            {
                // try to find existing DeviceEntity in DB, if it exists dont add it
                var dbDevice = Mapper.MapDevice(device);
                if (await _context.Roles.FindAsync(new object[] { dbDevice.Id }, cancellationToken: cancellationToken) is
                    not null) continue;
                await _context.AddAsync(dbDevice, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var queryRoles = new List<Device>();
            foreach (var device in devices)
            {
                queryRoles.Add(
                    Mapper.MapDevice(
                        await _context.Devices
                            .FindAsync(new object[] { device.Id }, cancellationToken: cancellationToken)));
            }
            return queryRoles.ToImmutableArray();
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }

    public async Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var numberOfSuccesses = 0;
        var deviceDeletionMessage = new List<string>();

        try
        {
            foreach (var deviceId in ids)
            {
                var dbDevice = await _context.Devices.FindAsync(
                    new object[] { deviceId }, cancellationToken: cancellationToken);

                if (dbDevice is null)
                {
                    //roleDeletionMessage.Add($"Role with guid {roleId} not found");
                    continue;
                }

                _context.Devices.Remove(dbDevice);
                numberOfSuccesses++;
                //roleDeletionMessage.Add("ok");
            }
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return (0, null);
        }
        finally
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return (numberOfSuccesses, deviceDeletionMessage);
    }

    public async Task<Device> UpdateAsync(Guid id, Device device, CancellationToken cancellationToken)
    {
        var dbDevice = Mapper.MapDevice(device);
        try
        {
            // find entity in db
            var entityFromDb = await _context.Devices
                .FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            // doesnt exist? there is nothing to update
            if (entityFromDb is null) return null;
            
            // update appropriate fields
            entityFromDb.UpdatedUtc = DateTime.UtcNow;
            entityFromDb.Name = dbDevice.Name;
            entityFromDb.IsDeleted = dbDevice.IsDeleted;
            entityFromDb.DeviceType = dbDevice.DeviceType;

            await _context.SaveChangesAsync(cancellationToken);

            return Mapper.MapDevice(
                await _context.Devices
                    .FindAsync(new object[] { id }, cancellationToken: cancellationToken));
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }
}