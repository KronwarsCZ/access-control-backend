using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Framework.Utilities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class KeyRepository : IKeyRepository
{
	public KeyRepository(AccessControlDbContext context)
	{
		_context = context;
	}
    
	private readonly AccessControlDbContext _context;
	
    // get all ids that can open a device
	public async Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
	{
        var query = await _context.Devices
            .Join(_context.DevicesRoles, d => d.Id, dr => dr.DeviceFId, (d, dr) => new { d, dr })
            .Join(_context.RolesKeys, @t => @t.dr.RoleFId, rk => rk.RoleFId, (@t, rk) => new { @t, rk })
            .Join(_context.Keys, @t => @t.rk.KeyFId, k => k.Id, (@t, k) => new { @t, k })
            .Where(@t => @t.@t.@t.d.Id == id)
            .Select(@t => t.k)
            .ToArrayAsync(cancellationToken);
        return query.Select(Mapper.MapKey);
    }

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        var query = await _context.Keys.ToArrayAsync(cancellationToken);
        return query.Select(Mapper.MapKey);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        var dbKeys = keys.Select(Mapper.MapKey).ToImmutableArray();
        await _context.Keys.AddRangeAsync(dbKeys, cancellationToken);
        return dbKeys.Select(k => k.Id);
    }

    public void Delete(IEnumerable<Key> keys)
    {
        var dbKeys = keys.Select(Mapper.MapKey);
        _context.Keys.RemoveRange(dbKeys);
    }

    public void Update(IEnumerable<Key> keys)
    {
        var dbKeys = keys.Select(Mapper.MapKey);
        _context.Keys.UpdateRange(dbKeys);
    }
}