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
}
