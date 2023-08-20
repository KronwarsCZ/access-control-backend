using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork context;
    
    public AccountService(IUnitOfWork context)
    {
        this.context = context;
    }

    public async Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        if (account.IsNotNull()) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var owner = Core.Entities.Owner.Create("my name");
        context.OwnerCommandRepository.Add(owner);

        var user = new Account
        {
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = context.TimeService.GetUtcTime(),
            UpdatedUtc = context.TimeService.GetUtcTime(),
            IsDeleted = false,
            Owner = owner,
            OwnerId = owner.Id,
        };


        context.AccountCommandRepository.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        // send confirmation email..

        return true;
    }
    


    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await context.AccountQueryRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }
}
