using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    public AccountRepository(AccessControlDbContext context, TimeFreezeService timeService)
    {
        this.timeService = timeService;
        this.context = context;
    }
    private readonly TimeFreezeService timeService;

    private readonly AccessControlDbContext context;


    public async Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken)
    {
        var userEntities = await context.Accounts.ToListAsync(cancellationToken);
        return userEntities.Select(Mapper.MapAccount);
    }

    public async Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res = await context.Accounts.FindAsync(id, cancellationToken);
        return Mapper.MapAccount(res);
    }

    public async Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var res = await context.Accounts.Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        return res.MapRef(r => Mapper.MapAccount(res));
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        var userEntities = users.Select(Mapper.MapAccount).ToList();
        context.Accounts.AddRange(userEntities);
        return userEntities.Select(d => d.Id);
    }

    public Task<IEnumerable<Account>> GetAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Guid>> AddAsync(IEnumerable<Account> entitiesToAdd, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<Account> entitiesToDelete, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Update(IEnumerable<Account> entitiesToUpdate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
