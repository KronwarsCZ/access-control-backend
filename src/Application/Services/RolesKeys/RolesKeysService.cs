using Application.IRepositories;
using Application.Services.Time;
using Core.Entities;

namespace Application.Services.RolesKeys;

public class RolesKeysService : IRolesKeysService
{
    private readonly IRolesKeysRepository RolesKeysRepository;
    private readonly TimeFreezeService TimeService;
    private readonly IDBContextSaveRepository DbContextSaveRepository;

    public RolesKeysService(IRolesKeysRepository rolesKeysRepository, TimeFreezeService timeService, IDBContextSaveRepository dbContextSaveRepository)
    {
        RolesKeysRepository = rolesKeysRepository;
        TimeService = timeService;
        DbContextSaveRepository = dbContextSaveRepository;
    }

    public async Task<IEnumerable<Key>> GetAtoBAsync(Role role, CancellationToken cancellationToken)
    {
        return await RolesKeysRepository.GetAtoBAsync(role, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetBtoAAsync(Key key, CancellationToken cancellationToken)
    {
        return await RolesKeysRepository.GetBtoAAsync(key, cancellationToken);
    }

    public async Task<Guid> AddAtoBAsync(Role role, IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        var id = await RolesKeysRepository.AddAtoBAsync(role, keys, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return id;
    }

    public async Task<Guid> AddBtoAAsync(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var id = await RolesKeysRepository.AddBtoAAsync(key, roles, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return id;
    }

    public async Task DeleteAtoBAsync(Role role, IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        RolesKeysRepository.DeleteAtoB(role, keys);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    public async Task DeleteBtoAAsync(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        RolesKeysRepository.DeleteBtoA(key, roles);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }
}
