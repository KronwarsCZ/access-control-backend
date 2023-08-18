using System.Collections.Immutable;
using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<IEnumerable<Owner>> GetAsync(CancellationToken cancellationToken)
    {
        var dbOwners =  await context.Owners.AsNoTracking().ToListAsync(cancellationToken);
        return dbOwners.Select(Mapper.Map);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Owner> owners, CancellationToken cancellationToken)
    {
        var dbOwners = owners.Select(Mapper.Map).ToImmutableList();
        await context.Owners.AddRangeAsync(dbOwners, cancellationToken);
        return dbOwners.Select(k => k.Id);
    }

    public void Delete(IEnumerable<Owner> entitiesToDelete)
    {
        throw new NotImplementedException();
    }

    public void Update(IEnumerable<Owner> entitiesToUpdate)
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
