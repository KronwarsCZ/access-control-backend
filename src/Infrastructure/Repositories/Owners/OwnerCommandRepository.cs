using Application.IRepositories.Owners;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories.Owners;

public class OwnerCommandRepository : IOwnerCommandRepository
{
        
    private readonly AccessControlDbContext context;
    public OwnerCommandRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public void Add(Owner owner)
    {
        context.Owners.Add(Mapper.Map(owner));
    }
}
