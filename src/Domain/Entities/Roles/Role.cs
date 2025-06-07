using Core.Entities.BaseEntities;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;

namespace Core.Entities.Roles;

public class Role : OwnedEntity
{
    private Role()
    {
    }

    public string Name { get; set; }
    public List<Device> Devices { get; } = new();

	public List<Key> Keys { get; } = new();

    public static Role Create(Owner owner, string name)
    {
        var role = CreateEntity<Role>();
        role.Name = name;
        role.Owner = owner;
        role.OwnerId = owner.Id;
        return role;
    }
}