using Application.IRepositories;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    public AccountRepository(AccessControlDbContext context)
    {
        _context = context;
    }

    private readonly AccessControlDbContext _context;

    public async Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken)
    {
        var userEntities = await _context.Accounts.ToListAsync(cancellationToken);
        return userEntities.Select(Mapper.MapAccount);
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res = await _context.Accounts.FindAsync(id, cancellationToken);
        return Mapper.MapAccount(res);
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var res = await _context.Accounts.Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        return res.MapRef(r => Mapper.MapAccount(res));
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        var userEntities = users.Select(Mapper.MapAccount).ToList();
        _context.Accounts.AddRange(userEntities);
        return userEntities.Select(d => d.Id);
    }
}
