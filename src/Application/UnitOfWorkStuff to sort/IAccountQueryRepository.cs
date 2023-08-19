using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.IRepositories;

public interface IAccountQueryRepository : IBasicQueryRepository<Account>
{ 
    Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
