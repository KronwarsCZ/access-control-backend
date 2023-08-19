using Application.IRepositories;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Repositories.UnitOfWork.BasicRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountQueryRepository : BasicQueryRepository<Account, AccountEntity>, IAccountQueryRepository
{
    public AccountQueryRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var accountEntity =  await Context.Accounts.Where(a => a.Email == email).FirstOrDefaultAsync(cancellationToken);
        return _mapper.Map<Account>(accountEntity);
    }
}
