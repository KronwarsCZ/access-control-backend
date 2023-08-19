using Application.IRepositories;
using Core.Entities;

namespace Application.Services.Accounts;

public interface IAccountService
{
    Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken);
    public bool IsPasswordCorrect(string password, Account account);
    public Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken);
}
