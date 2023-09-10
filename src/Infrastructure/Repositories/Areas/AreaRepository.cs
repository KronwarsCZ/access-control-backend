using Application.IRepositories.Areas;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Areas;

public class AreaRepository  : BasicRepository<Area>, IAreaRepository
{
        
    private readonly AccessControlDbContext _context;
    public AreaRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }
}
