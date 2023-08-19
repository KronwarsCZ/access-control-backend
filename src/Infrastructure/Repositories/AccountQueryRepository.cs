using Application.IRepositories;
using Application.Services;
using Application.Services.Time;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
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
        return accountEntity.MapRef(e => e.Map());
    }
}
