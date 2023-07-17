using Application.IRepositories;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public UserRepository(AccessControlDbContext context)
    {
        _context = context;
    }

    private readonly AccessControlDbContext _context;

    public async Task<IEnumerable<Account>> GetAsync(UserFilters filters, CancellationToken cancellationToken)
    {
        var userEntities = await _context.Users.ToListAsync(cancellationToken);
        return userEntities.Select(Mapper.MapUser);
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res = await _context.Users.FindAsync(id, cancellationToken);
        return Mapper.MapUser(res);
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var res = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        return res.MapRef(r => Mapper.MapUser(res));
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        var userEntities = users.Select(Mapper.MapUser).ToList();
        _context.Users.AddRange(userEntities);
        return userEntities.Select(d => d.Id);
    }
}
