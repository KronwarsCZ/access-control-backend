using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IAccountService
{
    void RegisterNewAccount(string email, string password);
    Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken);
    public bool IsPasswordCorrect(string password, Account account);
    public IEnumerable<Guid> Add(IEnumerable<Account> users);
    public Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken);
}
