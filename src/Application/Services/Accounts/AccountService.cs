using Application.Services.Owners;
using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : BasicService, IAccountService
{
    public AccountService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
    public async Task AddAccountUnderOwner(Owner owner, string email, string password, CancellationToken cancellationToken)
    {
        Check.Contract(owner, "Owner cannot be null");
        Check.Contract(!string.IsNullOrWhiteSpace(password), "Password cannot be null nor whitespace");

        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        Check.Contract(account is null, "Account with such email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var accountToAdd = Account.Create(owner, email, AccountRole.User, passwordHash);

        UnitOfWork.AccountRepository.Add(accountToAdd.ToEnumerable());

        // send confirmation email..
    }

    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await UnitOfWork.AccountRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }
}
