using Application.Services.BasicService;
using Core.Entities.Accounts;
using Core.Entities.Owners;

namespace Application.Services.Accounts;

public interface IAccountService : IBasicService
{
    public Task AddAccountUnderOwner(Owner owner, string email, string password, CancellationToken cancellationToken, AccountRole role = AccountRole.Admin);
    public bool IsPasswordCorrect(string password, Account account);
    public Task<Account?> GetAccountByEmailAsync(string email, CancellationToken cancellationToken);
    public Task ChangePasswordAsync(Account account, string currentPassword, string newPassword, CancellationToken cancellationToken);
    public Task<IEnumerable<Account>> GetAccountsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task DeleteAccountAsync(Account account, CancellationToken cancellationToken);
}
