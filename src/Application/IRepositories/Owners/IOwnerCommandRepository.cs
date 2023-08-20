using Core.Entities;

namespace Application.IRepositories.Owners;

public interface IOwnerCommandRepository
{
    void Add(Owner owner);
}
