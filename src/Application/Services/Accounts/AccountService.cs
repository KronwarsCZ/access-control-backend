using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services.Accounts;

public class AccountService : BasicService.BasicService, IAccountService
{
    private const int BCRYPT_WORK_FACTOR = 11;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);
    private readonly IMemoryCache _cache;

    public AccountService(IUnitOfWork.IUnitOfWork unitOfWork, IMemoryCache cache)
        : base(unitOfWork)
    {
        _cache = cache;
    }
    
    public async Task AddAccountUnderOwner(Owner owner, string email, string password, CancellationToken cancellationToken, AccountRole role = AccountRole.Admin)
    {
        Check.Contract(owner, "Owner cannot be null");
        Check.Contract(!string.IsNullOrWhiteSpace(password), "Password cannot be null nor whitespace");

        Check.Contract(email, "Email cannot be empty");
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");
        Check.Contract(RegexUtilities.IsPasswordValid(password),
            "Entered registration password is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        Check.Contract(account is null, "Account with such email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: BCRYPT_WORK_FACTOR);

        var accountToAdd = Account.Create(owner, email, role, passwordHash);

        UnitOfWork.AccountRepository.Add(accountToAdd.ToEnumerable());

        // TODO: send confirmation email..
    }

    public async Task<Account?> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");

        var cacheKey = $"account:{email}";
        if (_cache.TryGetValue<Account>(cacheKey, out var cached))
            return cached!;

        var account = await UnitOfWork.AccountRepository.GetByEmailAsync(email, cancellationToken);
        if (account is not null)
            _cache.Set(cacheKey, account, CacheDuration);

        return account;
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
        }
        catch (Exception ex) when (ex is BCrypt.Net.SaltParseException or ArgumentException)
        {
            // invalid hash from db or wrong arguments
            return false;
        }
    }

    public async Task<IEnumerable<Account>> GetAccountsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var accounts = await UnitOfWork.AccountRepository.FindByConditionAsync(
            a => a.OwnerId == ownerId && !a.IsDeleted, cancellationToken);
        return accounts;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.AccountRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAccountAsync(Account account, CancellationToken cancellationToken)
    {
        Check.Contract(account, "Account cannot be null");
        Check.Contract(account.Role != AccountRole.Owner, "Cannot delete the owner account");
        account.IsDeleted = true;
        account.TokenVersion++;
        UnitOfWork.AccountRepository.Update(account.ToEnumerable());
        await SaveChangesAsync(cancellationToken);
        _cache.Remove($"account:{account.Email}");
    }

    public async Task ChangePasswordAsync(Account account, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        Check.Contract(account, "Account cannot be null");
        Check.Contract(!string.IsNullOrWhiteSpace(currentPassword), "Current password cannot be empty");
        Check.Contract(!string.IsNullOrWhiteSpace(newPassword), "New password cannot be empty");
        Check.Contract(IsPasswordCorrect(currentPassword, account), "Current password is incorrect");
        Check.Contract(RegexUtilities.IsPasswordValid(newPassword),
            "New password must be at least 8 characters and contain an uppercase letter, lowercase letter, digit, and special character");

        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: BCRYPT_WORK_FACTOR);
        account.TokenVersion++;

        UnitOfWork.AccountRepository.Update(account.ToEnumerable());
        await SaveChangesAsync(cancellationToken);

        _cache.Remove($"account:{account.Email}");
    }
}
