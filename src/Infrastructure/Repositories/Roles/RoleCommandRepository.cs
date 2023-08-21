using Application.IRepositories.Roles;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories.Roles;

public class RoleCommandRepository : IRoleCommandRepository
{
    private readonly AccessControlDbContext context;
    public RoleCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public void Add(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken)
    {
        var dbRoles = rolesToAdd.Select(Mapper.Map);
        context.Roles.AddRange(dbRoles);
    }
}
