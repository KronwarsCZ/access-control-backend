using Core.Entities;

namespace Application.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAsync(UserFilters filters, CancellationToken cancellationToken);
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    IEnumerable<Guid> Add(IEnumerable<User> users);
}
