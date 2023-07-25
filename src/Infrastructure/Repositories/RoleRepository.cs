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
        try
        {
            var query = await _context.Roles.ToArrayAsync(cancellationToken);
            return query.Select(Mapper.MapRole).ToImmutableArray();
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Role>> AddAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var role in roles)
            {
                var dbRole = Mapper.MapRole(role);
                if (await _context.Roles.FindAsync(new object[] { dbRole.Id }, cancellationToken: cancellationToken) is
                    not null) continue;
                await _context.AddAsync(dbRole, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var queryRoles = new List<Role>();
            foreach (var role in roles)
            {
                queryRoles.Add(
                    Mapper.MapRole(
                        await _context.Roles
                            .FindAsync(new object[] { role.Id }, cancellationToken: cancellationToken)));
            }
            return queryRoles.ToImmutableArray();
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }

    public async Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var numberOfSuccesses = 0;
        var roleDeletionMessage = new List<string>();

        try
        {
            foreach (var roleId in ids)
            {
                var dbRole = await _context.Roles.FindAsync(
                    new object[] { roleId }, cancellationToken: cancellationToken);

                if (dbRole is null)
                {
                    //roleDeletionMessage.Add($"Role with guid {roleId} not found");
                    continue;
                }

                _context.Roles.Remove(dbRole);
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

        return (numberOfSuccesses, roleDeletionMessage);
    }

    public async Task<Role> UpdateAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        var dbRole = Mapper.MapRole(role);
        try
        {
            var entityFromDb = await _context.Roles
                .FindAsync(new object[] { id }, cancellationToken: cancellationToken);

            if (entityFromDb is null)
            {
                return null;
            }

            entityFromDb.UpdatedUtc = DateTime.UtcNow;
            entityFromDb.Name = dbRole.Name;
            entityFromDb.IsDeleted = dbRole.IsDeleted;

            await _context.SaveChangesAsync(cancellationToken);

            return Mapper.MapRole(
                await _context.Roles
                    .FindAsync(new object[] { id }, cancellationToken: cancellationToken));
        }
        catch (Exception e)
        {
            Check.Contract(false, e.Message);
            return null;
        }
    }
}
