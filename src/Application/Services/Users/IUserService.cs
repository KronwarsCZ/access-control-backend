using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public interface IUserService
{
    void RegisterNewUser(string email, string password);
    Task<IEnumerable<User>> GetAsync(UserFilters filters, CancellationToken cancellationToken);
}
