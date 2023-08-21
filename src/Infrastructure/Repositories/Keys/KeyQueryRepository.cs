using Application.IRepositories.Keys;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Keys;

public class KeyQueryRepository : IKeyQueryRepository
{
    private readonly AccessControlDbContext context;
    public KeyQueryRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
    
    // get all ids that can open a device
    public async Task<IEnumerable<Key>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = await context.Devices
            .Join(context.DevicesRoles, d => d.Id, dr => dr.DeviceFId, (d, dr) => new { d, dr })
            .Join(context.RolesKeys, @t => @t.dr.RoleFId, rk => rk.RoleFId, (@t, rk) => new { @t, rk })
            .Join(context.Keys, @t => @t.rk.KeyFId, k => k.Id, (@t, k) => new { @t, k })
            .Where(@t => @t.@t.@t.d.Id == id)
            .Select(@t => t.k)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
        return query.Select(Mapper.Map);
    }

    public async Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbKeys =  await context.Keys.ToListAsync(cancellationToken);
        return dbKeys.Select(Mapper.Map);
    }
}
