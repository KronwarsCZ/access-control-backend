using Application.IRepositories.Keys;
using Core.Entities.Keys;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Keys;

public class KeyRepository : BasicRepository<Key>, IKeyRepository
{
    public KeyRepository(AccessControlDbContext context)
        : base(context)
    { }

    public new async Task<Key> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Context.Keys.Where(k => k.Id == id).Include(k => k.Roles).FirstOrDefaultAsync(cancellationToken);
    }
}
