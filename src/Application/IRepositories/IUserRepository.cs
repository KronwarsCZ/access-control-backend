using Core.Entities;

namespace Application.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<Account>> GetAsync(UserFilters filters, CancellationToken cancellationToken);
    Task<Account> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
    IEnumerable<Guid> Add(IEnumerable<Account> users);
}
