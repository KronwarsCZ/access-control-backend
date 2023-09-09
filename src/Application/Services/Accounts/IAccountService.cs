using Core.Entities;
using Core.Entities.Accounts;
using Core.Entities.Owners;

namespace Application.Services.Accounts;

public interface IAccountService
{
    Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken, Owner owner = null);
    public bool IsPasswordCorrect(string password, Account account);
    public Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken);
}
