using Application.Services.Owners;
using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Identification;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork.IUnitOfWork unitOfWork;
    private readonly IOwnerService ownerService;
    
    public AccountService(IUnitOfWork.IUnitOfWork unitOfWork, IOwnerService ownerService)
    {
        this.unitOfWork = unitOfWork;
        this.ownerService = ownerService;
    }

    public async Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken, Owner owner = null)
    {
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        if (account.IsNotNull()) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        if (owner is null)
        {
            owner = Owner.Create(email);
            await ownerService.RegisterNewOwnerAsync(owner, cancellationToken);
        }

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
