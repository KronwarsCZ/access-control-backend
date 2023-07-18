using Application.IRepositories;
using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITimeService _timeService;

    public AccountService(
        IAccountRepository accountRepository,
        ITimeService timeService)
    {
        _accountRepository = accountRepository;
        _timeService = timeService;
    }

    public async Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        if (account.IsNotNull()) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new Account
        {
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = _timeService.GetUtcTime(),
            UpdatedUtc = _timeService.GetUtcTime(),
            IsDeleted = false,
            Owner = null,
            Name = null
        };

        _accountRepository.Add(user.ToEnumerable());
        // send confirmation email..

        return true;
    }

    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
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
