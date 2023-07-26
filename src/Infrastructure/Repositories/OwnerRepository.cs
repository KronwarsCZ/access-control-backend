using System.Collections.Immutable;
using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;

namespace Infrastructure.Repositories;

public class OwnerRepository : IOwnerRepository
{
    public OwnerRepository(AccessControlDbContext context, TimeFreezeService timeService)
    {
        this.timeService = timeService;
        this.context = context;
    }
    private readonly TimeFreezeService timeService;

    private readonly AccessControlDbContext context;
    
    public Task<IEnumerable<Owner>> GetAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Owner> owners, CancellationToken cancellationToken)
    {
        var dbOwners = owners.Select(Mapper.MapOwner).ToImmutableList();
        await context.Owners.AddRangeAsync(dbOwners, cancellationToken);
        return dbOwners.Select(k => k.Id);
    }

    public void Delete(IEnumerable<Owner> entitiesToDelete, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Update(IEnumerable<Owner> entitiesToUpdate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Owner Create()
    {
        throw new NotImplementedException();
    }

    public Owner Create(string name)
    {
        return new Owner
        {
            Name = name,
            CreatedUtc = timeService.GetUtcTime(),
            UpdatedUtc = timeService.GetUtcTime(),
        };
    }
}
