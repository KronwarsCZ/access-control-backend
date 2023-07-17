using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IUserService
{
    void RegisterNewUser(string email, string password);
    Task<IEnumerable<Account>> GetAsync(UserFilters filters, CancellationToken cancellationToken);
    public bool IsPasswordCorrect(string password, Account account);
    public IEnumerable<Guid> Add(IEnumerable<Account> users);
    public Task<Account> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}
