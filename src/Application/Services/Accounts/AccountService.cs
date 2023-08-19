using Application.IRepositories;
using Application.Services.Time;
using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly TimeFreezeService timeService;
    private readonly IDBContextSaveRepository dbContextSaveRepository;
    private readonly IOwnerRepository ownerRepository;
    private readonly IAccountQueryRepository accountQueryRepository;
    private readonly IAccountCommandRepository accountCommandRepository;

    public AccountService(
        IOwnerRepository ownerRepository,
        TimeFreezeService timeService,
        IDBContextSaveRepository dbContextSaveRepository,
        IAccountQueryRepository accountQueryRepository)
    {
        this.dbContextSaveRepository = dbContextSaveRepository;
        this.accountQueryRepository = accountQueryRepository;
        this.timeService = timeService;
        this.ownerRepository = ownerRepository;
    }

    public async Task<bool> RegisterNewAccount(string email, string password, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();

        Check.Contract(RegexUtilities.IsEmailValid(email), "Entered registration email is not in correct format");

        var account = await GetAccountByEmailAsync(email, cancellationToken);

        // account already exists
        if (account.IsNotNull()) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var owner = ownerRepository.Create("my name");
        await ownerRepository.AddAsync(owner.ToEnumerable(), cancellationToken);
        await dbContextSaveRepository.SaveContextAsync(cancellationToken);

        var user = new Account
        {
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = timeService.GetUtcTime(),
            UpdatedUtc = timeService.GetUtcTime(),
            IsDeleted = false,
            Owner = owner,
            OwnerId = owner.Id,
        };


        accountCommandRepository.Add(user);
        await dbContextSaveRepository.SaveContextAsync(cancellationToken);
        // send confirmation email..

        return true;
    }
    


    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await accountQueryRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }
}
