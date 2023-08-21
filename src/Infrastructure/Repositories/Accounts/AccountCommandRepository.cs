using Application.IRepositories.Accounts;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories.Accounts;

public class AccountCommandRepository : IAccountCommandRepository
{
    private readonly AccessControlDbContext context;
    public AccountCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public void Add(Account user)
    {
        context.Accounts.Add(Mapper.Map(user));
    }
}
