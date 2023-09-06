using Application.IRepositories.Accounts;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Accounts;

public class AccountRepository : BasicRepository<Account>, IAccountRepository
{
        
    private readonly AccessControlDbContext _context;
    public AccountRepository(AccessControlDbContext _context)
        : base(_context)
    {
        this._context = _context;
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Accounts.SingleOrDefaultAsync(a => a.Email == email, cancellationToken: cancellationToken);
    }
}
