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

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            TwoFactorAuthenticationEnabled = false
        };
        _userRepository.Add(user.ToEnumerable());
        // send confirmation email..
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        Check.Contract(RegexUtilities.IsValidEmail(email), $"Entered registration email is not in correct format");
        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public IEnumerable<Guid> Add(IEnumerable<User> users)
    {
        return _userRepository.Add(users);
    }

    public async Task<IEnumerable<User>> GetAsync(UserFilters filters, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAsync(filters, cancellationToken);
    }
}
