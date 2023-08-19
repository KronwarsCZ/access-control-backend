using Application.IRepositories;
using Application.Services.Time;
using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly IAccountRepository accountRepository;
    private readonly TimeFreezeService timeService;
    private readonly IDBContextSaveRepository dbContextSaveRepository;
    private readonly IOwnerRepository ownerRepository;

    public AccountService(
        IAccountRepository accountRepository,
        IOwnerRepository ownerRepository,
        TimeFreezeService timeService,
        IDBContextSaveRepository dbContextSaveRepository)
    {
        this.dbContextSaveRepository = dbContextSaveRepository;
        this.accountRepository = accountRepository;
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


        accountRepository.Add(user.ToEnumerable());
        await dbContextSaveRepository.SaveContextAsync(cancellationToken);
        // send confirmation email..

        return true;
    }
    


    public async Task<Account> GetAccountByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();
        Check.Contract(RegexUtilities.IsEmailValid(email), $"Entered registration email is not in correct format");
        return await accountRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        return accountRepository.Add(users);
    }

    public async Task<IEnumerable<Account>> GetAsync(AccountFilters filters, CancellationToken cancellationToken)
    {
        return await accountRepository.GetAsync(filters, cancellationToken);
    }
}
