using Core.Entities.Devices;
using Core.Entities.Roles;

namespace Core.Entities.JoinEntities;

public sealed class DeviceRole
{
    public Guid DeviceId { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Device Device { get; set; } = null!;
}
