using Application.IRepositories.Roles;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Roles;

public class RoleQueryRepository : IRoleQueryRepository
{
    private readonly AccessControlDbContext context;
    public RoleQueryRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbEntities = await context.Roles.ToListAsync(cancellationToken);
        return dbEntities.Select(Mapper.Map);
    }

    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbRole = await context.Roles.FindAsync( new object[] {id}, cancellationToken);
        return Mapper.Map(dbRole);
    }

    public void Delete(IEnumerable<Role> roles)
    {
        context.Roles.RemoveRange(roles.Select(Mapper.Map));
    }
}
