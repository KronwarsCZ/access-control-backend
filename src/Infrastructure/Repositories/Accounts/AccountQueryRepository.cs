using Application.IRepositories.Accounts;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Accounts;

public class AccountQueryRepository : IAccountQueryRepository
{
    private readonly AccessControlDbContext context;
    public AccountQueryRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var accountEntity =  await context.Accounts.Where(a => a.Email == email).FirstOrDefaultAsync(cancellationToken);
        return accountEntity.MapRef(Mapper.Map);
    }
}
