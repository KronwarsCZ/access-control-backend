using Application.IRepositories;
using Application.Utilities;
using Core.Entities;
using Framework.Extensions;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public void RegisterNewAccount(string email, string password)
    {
        Check.Contract(RegexUtilities.IsValidEmail(email), $"Entered registration email is not in correct format");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new Account
        {
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false,
            Owner = null,
            Name = null
        };
        _accountRepository.Add(user.ToEnumerable());
        // send confirmation email..
    }

    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        Check.Contract(RegexUtilities.IsValidEmail(email), $"Entered registration email is not in correct format");
        return await _accountRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        return _accountRepository.Add(users);
    }

    public async Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetAsync(filters, cancellationToken);
    }
}
