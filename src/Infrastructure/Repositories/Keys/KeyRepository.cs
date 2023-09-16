using Application.IRepositories.Keys;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Keys;

public class KeyRepository : BasicRepository<Key>, IKeyRepository
{
    public KeyRepository(AccessControlDbContext context)
        : base(context)
    { }

    public new async Task<Key> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Keys.Where(k => k.Id == id).Include(k => k.Roles).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetDevicesToOpen(Guid keyId, CancellationToken cancellationToken)
    {
        var key =  await Context.Keys
            .Where(e => e.Id == keyId && e.IsDeleted == false)
            .Include(d => d.Roles)
            .ThenInclude(r => r.Devices)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return key.Roles.SelectMany(r => r.Devices);
    }
}
