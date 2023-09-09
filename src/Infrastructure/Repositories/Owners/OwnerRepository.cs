using Application.IRepositories.Owners;
using Core.Entities;
using Core.Entities.Owners;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Owners;

public class OwnerRepository : BasicRepository<Owner>, IOwnerRepository
{
    public OwnerRepository(AccessControlDbContext context)
        : base(context)
    {
    }
}
