using Application.IRepositories.Owners;
using Core.Entities.Owners;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Owners;

public class OwnerRepository : BasicRepository<Owner>, IOwnerRepository
{
    public OwnerRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public new async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return (await Context.Owners.Where(o => o.Id == id && o.IsDeleted == false).Include(o => o.Accounts)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken))!;
    }
}
