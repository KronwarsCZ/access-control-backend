using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
using Infrastructure.Repositories.UnitOfWork.BasicRepositories;

namespace Infrastructure.Repositories;

public class AccountCommandRepository : BasicCommandRepository<Account, AccountEntity>, IAccountCommandRepository
{
    public AccountCommandRepository(AccessControlDbContext context) : base(context)
    {
    }
}
