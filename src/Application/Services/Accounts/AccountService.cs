using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;
using Framework.Identification;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork unitOfWork;
    
    public AccountService(IUnitOfWork context)
    {
        this.unitOfWork = context;
    }

    public async Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        if (account.IsNotNull()) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var owner = Owner.Create("my name");

        var user = new Account
        {
            Id = SequentialGuid.Create(),
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = unitOfWork.TimeService.GetUtcTime(),
            UpdatedUtc = unitOfWork.TimeService.GetUtcTime(),
            IsDeleted = false,
            Owner = owner,
        };


        unitOfWork.AccountRepository.Add(user.ToEnumerable());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        // send confirmation email..

        return true;
    }

    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await unitOfWork.AccountRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }
}
