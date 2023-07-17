using Application.IRepositories;
using Application.Utilities;
using Core.Entities;
using Framework.Extensions;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void RegisterNewUser(string email, string password)
    {
        Check.Contract(RegexUtilities.IsValidEmail(email), $"Entered registration email is not in correct format");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = AccountRole.User,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false,
            CreatedUtc = default,
            UpdatedUtc = default,
            IsDeleted = false,
            Owner = null
        };
        _userRepository.Add(user.ToEnumerable());
        // send confirmation email..
    }

    public async Task<Account> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        Check.Contract(RegexUtilities.IsValidEmail(email), $"Entered registration email is not in correct format");
        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public bool IsPasswordCorrect(string password, Account account)
    {
        return BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
    }

    public IEnumerable<Guid> Add(IEnumerable<Account> users)
    {
        return _userRepository.Add(users);
    }

    public async Task<IEnumerable<Account>> GetAsync(UserFilters filters, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAsync(filters, cancellationToken);
    }
}
