using Core.Entities.BaseEntities;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;

namespace Core.Entities.Roles;

public class Role : Entity
{
    private Role()
    {
    }

    public string Name { get; set; }
    public Owner Owner { get; set; }
    public Guid OwnerId { get; }
    public List<Device> Devices { get; } = new();

	public List<Key> Keys { get; } = new();

    public static Role Create(Owner owner, string name)
    {
        var role = CreateEntity<Role>();
        role.Name = name;
        role.Owner = owner;
        return role;
    }
}