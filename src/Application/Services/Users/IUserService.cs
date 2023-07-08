using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IUserService
{
    void RegisterNewUser(string email, string password);
    Task<IEnumerable<User>> GetAsync(UserFilters filters, CancellationToken cancellationToken);
    public bool IsPasswordCorrect(string password, User user);
    public IEnumerable<Guid> Add(IEnumerable<User> users);
    public Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}
