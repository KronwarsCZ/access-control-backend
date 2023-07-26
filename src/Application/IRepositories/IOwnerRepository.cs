using Core.Entities;

namespace Application.IRepositories;

public interface IOwnerRepository : IBasicRepository<Owner>
{
    public Owner Create(string name);
}