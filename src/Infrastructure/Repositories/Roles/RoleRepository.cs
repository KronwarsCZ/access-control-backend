using Application.IRepositories.Roles;
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

    public new async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return (await Context.Roles
            .Where(r => r.Id == id && r.IsDeleted == false)
            .Include(r => r.Keys)
            .Include(r => r.Devices)
            .FirstOrDefaultAsync(cancellationToken))!;
    }
}
