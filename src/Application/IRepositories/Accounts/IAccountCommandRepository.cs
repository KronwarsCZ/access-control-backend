using Core.Entities;

namespace Application.IRepositories.Accounts;

public interface IAccountCommandRepository
{
    void Add(Account user);
}
