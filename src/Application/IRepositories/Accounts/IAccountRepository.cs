using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.IRepositories.Accounts;

public interface IAccountRepository : IBasicRepository<Account>
{
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
