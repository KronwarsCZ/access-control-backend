using Core.Entities;

namespace Application.IRepositories;

public interface IAccountQueryRepository
{ 
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
