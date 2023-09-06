using Application.IRepositories.Owners;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Owners;

public class OwnerRepository : BasicRepository<Owner>, IOwnerRepository
{
        
    private readonly AccessControlDbContext _context;
    public OwnerRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }
}
