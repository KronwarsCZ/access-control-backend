using Core.Entities;

namespace Application.IRepositories;

public interface IAccountRepository : IBasicRepository<Account>
{
    Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken);
    Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
    IEnumerable<Guid> Add(IEnumerable<Account> users);
}
