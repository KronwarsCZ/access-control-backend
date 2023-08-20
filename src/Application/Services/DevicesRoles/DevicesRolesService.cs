using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.DevicesRoles;

public class DevicesRolesService : IDevicesRolesService
{
    private readonly IUnitOfWork context;

    public DevicesRolesService(IUnitOfWork context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Role>> GetAtoBAsync(Device device, CancellationToken cancellationToken)
    {
        return await context.DevicesRolesRepository.GetAtoBAsync(device, cancellationToken);
    }
    

    public async Task<IEnumerable<Device>> GetBtoAAsync(Role role, CancellationToken cancellationToken)
    {
        return await context.DevicesRolesRepository.GetBtoAAsync(role, cancellationToken);
    }

    public async Task<Guid> AddAtoBAsync(Device device, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var id = await context.DevicesRolesRepository.AddAtoBAsync(device, roles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return id;
    }

    public async Task<Guid> AddBtoAAsync(Role role, IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var id = await context.DevicesRolesRepository.AddBtoAAsync(role, devices, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return id;
    }

    public async Task DeleteAtoBAsync(Device device, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        context.DevicesRolesRepository.DeleteAtoB(device, roles);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteBtoAAsync(Role role, IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        context.DevicesRolesRepository.DeleteBtoA(role, devices);
        await context.SaveChangesAsync(cancellationToken);
    }
}
