using Application.IRepositories.Roles;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Roles;

public class RoleRepository : BasicRepository<Role>, IRoleRepository
{
        
    private readonly AccessControlDbContext _context;
    public RoleRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }
    
    public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Roles.FindAsync( new object[] {id}, cancellationToken);
    }
}
