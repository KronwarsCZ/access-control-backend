using Application.IRepositories;
using Application.Services.Time;

namespace Application.Services.Owner;

public class OwnerService : IOwnerService
{
    private readonly IOwnerRepository ownerRepository;
    private readonly TimeFreezeService timeService;
    private readonly IDBContextSaveRepository dbContextSaveRepository;

    public OwnerService(IOwnerRepository ownerRepository, TimeFreezeService timeService, IDBContextSaveRepository dbContextSaveRepository)
    {
        this.ownerRepository = ownerRepository;
        this.timeService = timeService;
        this.dbContextSaveRepository = dbContextSaveRepository;
    }
    public Core.Entities.Owner Create(string name)
    {
        return new Core.Entities.Owner
        {
            Name = name,
            CreatedUtc = timeService.GetUtcTime(),
            UpdatedUtc = timeService.GetUtcTime()
        };
    }

    public Task<IEnumerable<Core.Entities.Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        return ownerRepository.GetAsync(cancellationToken);
    }

    public Task<IEnumerable<Guid>> AddAsync(IEnumerable<Core.Entities.Owner> entitiesToAdd, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Delete(IEnumerable<Core.Entities.Owner> entitiesToDelete, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(IEnumerable<Core.Entities.Owner> entitiesToUpdate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
