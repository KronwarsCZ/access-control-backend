using Application.IRepositories.Roles;
using Core.Entities.Roles;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Roles;

public class RoleRepository : BasicRepository<Role>, IRoleRepository
{
    public RoleRepository(AccessControlDbContext context)
        : base(context)
    {
    }
}
