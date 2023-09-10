using Core.Entities.Accounts;

namespace Application.IRepositories.Accounts;

public interface IAccountRepository : IBasicRepository<Account>
{
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
