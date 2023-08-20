using Core.Entities;

namespace Application.IRepositories.Accounts;

public interface IAccountQueryRepository
{ 
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
