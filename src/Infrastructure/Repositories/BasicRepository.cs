using Infrastructure.Database;

namespace Infrastructure.Repositories;

public abstract class BasicRepository
{
    protected readonly AccessControlDbContext _context;

    protected BasicRepository(AccessControlDbContext context)
    {
        _context = context;
    }
}
