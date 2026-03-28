using Application.IRepositories.Roles;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Roles;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Roles;

public class RoleRepository : BasicRepository<Role>, IRoleRepository
{
    public RoleRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Key>?> GetKeysForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken)
    {
        var roleOwned = await Context.Roles.AsNoTracking()
            .AnyAsync(r => r.Id == roleId && r.OwnerId == ownerId && !r.IsDeleted, cancellationToken);
        if (!roleOwned)
            return null;

        return await Context.Keys.AsNoTracking()
            .Where(k => !k.IsDeleted && k.OwnerId == ownerId)
            .Where(k => k.Roles.Any(r => r.Id == roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Device>?> GetDevicesForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken)
    {
        var roleOwned = await Context.Roles.AsNoTracking()
            .AnyAsync(r => r.Id == roleId && r.OwnerId == ownerId && !r.IsDeleted, cancellationToken);
        if (!roleOwned)
            return null;

        return await Context.Devices.AsNoTracking()
            .Where(d => !d.IsDeleted && d.OwnerId == ownerId)
            .Where(d => d.Roles.Any(r => r.Id == roleId))
            .ToListAsync(cancellationToken);
    }
}
