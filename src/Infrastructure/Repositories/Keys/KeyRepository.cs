using Application.IRepositories.Keys;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Keys;

public class KeyRepository : BasicRepository<Key>, IKeyRepository
{
        
    private readonly AccessControlDbContext _context;
    public KeyRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }
}
