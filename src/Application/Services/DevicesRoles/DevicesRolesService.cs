using Application.IRepositories;
using Application.Services.Time;
using Core.Entities;

namespace Application.Services.DevicesRoles;

public class DevicesRolesService : IDevicesRolesService
{
    private readonly IDevicesRolesRepository DevicesRolesRepository;
    private readonly TimeFreezeService TimeService;
    private readonly IDBContextSaveRepository DbContextSaveRepository;

    public DevicesRolesService(IDevicesRolesRepository devicesRolesRepository, TimeFreezeService timeService, IDBContextSaveRepository dbContextSaveRepository)
    {
        DevicesRolesRepository = devicesRolesRepository;
        TimeService = timeService;
        DbContextSaveRepository = dbContextSaveRepository;
    }

    public async Task<IEnumerable<Role>> GetAtoBAsync(Device device, CancellationToken cancellationToken)
    {
        return await DevicesRolesRepository.GetAtoBAsync(device, cancellationToken);
    }
    

    public async Task<IEnumerable<Device>> GetBtoAAsync(Role role, CancellationToken cancellationToken)
    {
        return await DevicesRolesRepository.GetBtoAAsync(role, cancellationToken);
    }

    public async Task<Guid> AddAtoBAsync(Device device, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var id = await DevicesRolesRepository.AddAtoBAsync(device, roles, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return id;
    }

    public async Task<Guid> AddBtoAAsync(Role role, IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var id = await DevicesRolesRepository.AddBtoAAsync(role, devices, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return id;
    }

    public async Task DeleteAtoBAsync(Device device, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        DevicesRolesRepository.DeleteAtoB(device, roles);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    public async Task DeleteBtoAAsync(Role role, IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        DevicesRolesRepository.DeleteBtoA(role, devices);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }
}
