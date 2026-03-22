using Core.Entities.Keys;
using Core.Entities.Roles;

namespace Core.Entities.JoinEntities;

public class RoleKey
{
    public Guid RoleId { get; set; }
    public Guid KeyId { get; set; }
    public Role Role { get; set; } = null!;
    public Key Key { get; set; } = null!;
}
