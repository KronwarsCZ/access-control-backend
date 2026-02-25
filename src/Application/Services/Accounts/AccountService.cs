using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : BasicService.BasicService, IAccountService
{
    private const int BCRYPT_WORK_FACTOR = 11;
    
    public AccountService(IUnitOfWork.IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
    
    public async Task AddAccountUnderOwner(Owner owner, string email, string password, CancellationToken cancellationToken)
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

        var accountToAdd = Account.Create(owner, email, AccountRole.User, passwordHash);

        UnitOfWork.AccountRepository.Add(accountToAdd.ToEnumerable());

        // TODO: send confirmation email..
    }

    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await UnitOfWork.AccountRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<Account> GetAccountByJwtTokenAsync(string token, CancellationToken cancellationToken)
    {
        Check.Contract(token, $"Passed JWT token is empty");
        return (await UnitOfWork.AccountRepository.FindByConditionAsync(a => a.SessionToken == token, cancellationToken)).SingleOrDefault();    
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
}
