using System.Collections;
using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DevicesRolesRepository : IDevicesRolesRepository
{
    private readonly AccessControlDbContext _context;

    public DevicesRolesRepository(AccessControlDbContext context)
    {
        _context = context;
    }
    
    private class AToManyB<A, B> : IEnumerable<DbDevicesRoles>
        where A : Entity
        where B : Entity
    {
        private readonly A _one;
        private readonly IEnumerable<B> _manyEnumerable;

        internal AToManyB(A one, IEnumerable<B> manyEnumerable)
        {
            _one = one;
            _manyEnumerable = manyEnumerable;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<DbDevicesRoles> GetEnumerator()
        {
            foreach (var elementFromMany in _manyEnumerable)
            {
                switch (_one)
                {
                    case Device one:
                        yield return new DbDevicesRoles()
                        {
                            DeviceFId = one.Id,
                            DeviceEntity = Mapper.Map(one),

                            RoleFId = elementFromMany.Id,
                            RoleEntity = Mapper.Map(elementFromMany as Role)
                        };
                        break;
                    case Role one:
                        yield return new DbDevicesRoles()
                        {
                            DeviceFId = elementFromMany.Id,
                            DeviceEntity = Mapper.Map(elementFromMany as Device),

                            RoleFId = one.Id,
                            RoleEntity = Mapper.Map(one)
                        };
                        break;
                }
            }
        }
    }
    
    public async Task<IEnumerable<Role>> GetAtoBAsync(Device device, CancellationToken cancellationToken)
    {
        var query = await _context.DevicesRoles
            .Where(dr => dr.DeviceFId == device.Id)
            .Include(dr => dr.RoleEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allRoles = query.Select(dr => dr.RoleEntity).ToArray();

        return allRoles.Select(Mapper.Map).ToImmutableArray();

    }

    public async Task<IEnumerable<Device>> GetBtoAAsync(Role role, CancellationToken cancellationToken)
    {
        var query = await _context.DevicesRoles
            .Where(dr => dr.RoleFId == role.Id)
            .Include(dr => dr.DeviceEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allDevices = query.Select(dr => dr.DeviceEntity).ToArray();

        return allDevices.Select(Mapper.Map).ToImmutableArray();
    }

    

    public async Task<Guid> AddAtoBAsync(Device device, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var deviceToManyRoles = new AToManyB<Device, Role>(device, roles);
        await _context.DevicesRoles.AddRangeAsync(deviceToManyRoles, cancellationToken);
        return device.Id;
    }

    public async Task<Guid> AddBtoAAsync(Role role, IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var roleToManyDevices = new AToManyB<Role, Device>(role, devices);
        await _context.DevicesRoles.AddRangeAsync(roleToManyDevices, cancellationToken);
        return role.Id;
    }

    public void DeleteAtoB(Device device, IEnumerable<Role> roles)
    {
        var deviceToManyRoles = new AToManyB<Device, Role>(device, roles);
        _context.DevicesRoles.RemoveRange(deviceToManyRoles);
    }

    public void DeleteBtoA(Role role, IEnumerable<Device> devices)
    {
        var roleToManyDevices = new AToManyB<Role, Device>(role, devices);
        _context.DevicesRoles.RemoveRange(roleToManyDevices);

    }
}
