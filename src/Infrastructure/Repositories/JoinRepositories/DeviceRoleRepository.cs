using Application.IRepositories.JoinRepositories;
using Core.Entities.JoinEntities;
using Infrastructure.Database;

namespace Infrastructure.Repositories.JoinRepositories;

public class DeviceRoleRepository : IDeviceRoleRepository
{
    private readonly AccessControlDbContext _context;

    public DeviceRoleRepository(AccessControlDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsDeviceRoleConnectionExisting(Guid deviceId, Guid roleId, CancellationToken cancellationToken)
    {
        var connection = await _context.DeviceRole.FindAsync(deviceId, roleId);
        return connection is not null;
    }

    public void Add(IEnumerable<DeviceRole> deviceRoles)
    {
        _context.Add(deviceRoles);
    }

    public async Task RemoveAsync(Guid deviceId, Guid roleId, CancellationToken cancellationToken)
    {
        var connection = await _context.DeviceRole.FindAsync(deviceId, roleId);
        if (connection is not null)
            _context.DeviceRole.Remove(connection);
    }
}
