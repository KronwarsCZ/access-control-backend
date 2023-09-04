using Application.IRepositories.Owners;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Owners;

public class OwnerQueryRepository : IOwnerQueryRepository
{
    
    private readonly AccessControlDbContext context;
    public OwnerQueryRepository(AccessControlDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbOwners = await context.Owners.ToListAsync(cancellationToken);
        return dbOwners.Select(Mapper.Map);
    }
    
    public async Task<Owner> GetByIDAsync(Guid id,CancellationToken cancellationToken)
    {
        var dbOwners = await context.Owners.Where(entity => entity.Id.Equals(id)).ToListAsync(cancellationToken);
        return Mapper.Map(dbOwners[0]);
    }
}
