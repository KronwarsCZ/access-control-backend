using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Framework.Utilities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AccessControlDbContext _context;

    public RoleRepository(AccessControlDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        var query = await _context.Roles.ToArrayAsync(cancellationToken);
        return query.Select(Mapper.MapRole).ToImmutableArray();
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var dbRoles = roles.Select(Mapper.MapRole).ToImmutableArray();
        await _context.Roles.AddRangeAsync(dbRoles, cancellationToken);
        return dbRoles.Select(r => r.Id);
    }

    public void Delete(IEnumerable<Role> roles)
    {
        var dbRoles = roles.Select(Mapper.MapRole).ToImmutableArray();
        _context.Roles.RemoveRange(dbRoles);
    }

    public void Update(IEnumerable<Role> role)
    {
        var dbRoles = role.Select(Mapper.MapRole).ToImmutableArray();
        _context.Roles.UpdateRange(dbRoles);
    }
}