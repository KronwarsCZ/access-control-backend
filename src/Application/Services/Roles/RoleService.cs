using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository RoleRepository;
    private readonly TimeFreezeService TimeService;
    private readonly IDBContextSaveRepository DbContextSaveRepository;

    public RoleService(IRoleRepository roleRepository, TimeFreezeService timeFreezeService, IDBContextSaveRepository dbContextSaveRepository)
    {
        RoleRepository = roleRepository;
        TimeService = timeFreezeService;
        DbContextSaveRepository = dbContextSaveRepository;
    }

    public Role Create(string name)
    {
        return new Role()
        {
            CreatedUtc = TimeService.GetUtcTime(),
            UpdatedUtc = TimeService.GetUtcTime(),
            Name = name,
            IsDeleted = false
        };
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await RoleRepository.GetAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Role> things, CancellationToken cancellationToken)
    {
        var ids = await RoleRepository.AddAsync(things, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return ids;
    }

    public async Task Delete(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        RoleRepository.Delete(roles);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    public async Task Update(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        RoleRepository.Update(roles);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }
}
