using Application.IRepositories.Accounts;
using Core.Entities.Accounts;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Accounts;

public class AccountRepository : BasicRepository<Account>, IAccountRepository
{
    public AccountRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Accounts.Include(a => a.Owner).SingleOrDefaultAsync(a => a.Email == email, cancellationToken: cancellationToken);
    }
}
