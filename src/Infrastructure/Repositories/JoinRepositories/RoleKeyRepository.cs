using Application.IRepositories.JoinRepositories;
using Core.Entities.JoinEntities;
using Infrastructure.Database;

namespace Infrastructure.Repositories.JoinRepositories;

public class RoleKeyRepository : IRoleKeyRepository
{
    private readonly AccessControlDbContext _context;

    public RoleKeyRepository(AccessControlDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsRoleKeyConnectionExisting(Guid roleId, Guid keyId, CancellationToken cancellationToken)
    {
        var connection = await _context.RoleKey.FindAsync(roleId, keyId);
        return connection is not null;
    }

    public void Add(IEnumerable<RoleKey> roleKeys)
    {
        _context.Add(roleKeys);
    }

    public async Task RemoveAsync(Guid roleId, Guid keyId, CancellationToken cancellationToken)
    {
        var connection = await _context.RoleKey.FindAsync(roleId, keyId);
        if (connection is not null)
            _context.RoleKey.Remove(connection);
    }
}