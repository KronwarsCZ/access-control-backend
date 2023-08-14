using System.Collections.Immutable;
using System.Data.SqlTypes;
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

    public async Task<IEnumerable<Role>> GetAsync(CancellationToken cancellationToken)
    {
        var query = await _context.Roles.AsNoTracking().ToArrayAsync(cancellationToken);
        return query.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<Role> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = await _context.Roles.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (query is null)
        {
            throw new SqlNullValueException("The given primary key doesnt exist in database");
        }

        return Mapper.Map(query);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var dbRoles = roles.Select(Mapper.Map).ToImmutableArray();
        await _context.Roles.AddRangeAsync(dbRoles, cancellationToken);
        return dbRoles.Select(r => r.Id);
    }

    public void Delete(IEnumerable<Role> roles)
    {
        var dbRoles = roles.Select(Mapper.Map).ToImmutableArray();
        _context.Roles.RemoveRange(dbRoles);
    }

    public void Update(IEnumerable<Role> role)
    {
        var dbRoles = role.Select(Mapper.Map).ToImmutableArray();
        _context.Roles.UpdateRange(dbRoles);
    }
}