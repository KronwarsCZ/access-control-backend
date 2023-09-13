using Application.Services.BasicService;
using Core.Entities.Accounts;
using Core.Entities.Owners;

namespace Application.Services.Accounts;

public interface IAccountService : IBasicService
{
    public Task AddAccountUnderOwner(Owner owner, string email, string password, CancellationToken cancellationToken);
    public bool IsPasswordCorrect(string password, Account account);
    public Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken);
    public Task<Account> GetAccountByJwtTokenAsync(string token, CancellationToken cancellationToken);
}
