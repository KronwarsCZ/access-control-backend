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
        try
        {
            var query = await _context.Devices
                .Join(_context.DevicesRoles, d => d.Id, dr => dr.DeviceFId, (d, dr) => new { d, dr })
                .Join(_context.RolesKeys, @t => @t.dr.RoleFId, rk => rk.RoleFId, (@t, rk) => new { @t, rk })
                .Join(_context.Keys, @t => @t.rk.KeyFId, k => k.Id, (@t, k) => new { @t, k })
                .Where(@t => @t.@t.@t.d.Id == id)
                .Select(@t => t.k)
                .ToListAsync(cancellationToken);
            return query.Count == 0 ? null : query.Select(Mapper.MapKey).ToImmutableList();
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var query = await _context.Keys.ToListAsync(cancellationToken);
            return query.Select(Mapper.MapKey).ToImmutableList();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Key>> AddAsync(IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var key in keys)
            {
                //TODO: this throws error when adding key that already exists
                await _context.AddAsync(Mapper.MapKey(key), cancellationToken);    
            }
            await _context.SaveChangesAsync(cancellationToken);

            var queryKeys = new List<Key>();
            foreach (var key in keys)
            {
                queryKeys.Add(
                    Mapper.MapKey(
                        await _context.Keys.FindAsync(
                            new object[] { key.Id }, cancellationToken: cancellationToken)));
            }

            return queryKeys.ToImmutableList();
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
        var keyDeletionMessage = new List<string>();

        try
        {
            foreach (var keyId in ids)
            {
                var dbKey = await _context.Keys.FindAsync(keyId);
                if (dbKey is null)
                {
                    keyDeletionMessage.Add($"Key with guid {keyId} not found");
                    continue;
                }

                _context.Keys.Remove(dbKey);
                numberOfSuccesses++;
                keyDeletionMessage.Add("ok");
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

        return (numberOfSuccesses, keyDeletionMessage);
    }

    public async Task<Key> UpdateAsync(Guid keyId, Key key, CancellationToken cancellationToken)
    {
        var dbKey = Mapper.MapKey(key);
        var entityFromDb = await _context.Keys.FindAsync(
            new object[] { keyId }, cancellationToken: cancellationToken);

        if (entityFromDb is null)
        {
            return null;
        }
        
        // Changing anything else has no meaning now (July 2023)
        entityFromDb.UpdatedUtc = DateTime.UtcNow;
        entityFromDb.Pass = dbKey.Pass;
        entityFromDb.Name = dbKey.Name;
        entityFromDb.IsDeleted = dbKey.IsDeleted;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Mapper.MapKey(await _context.Keys.FindAsync(
            new object[] { keyId }, cancellationToken: cancellationToken));
    }
}