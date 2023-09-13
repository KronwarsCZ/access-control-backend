using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Core.Entities.Keys;

public class Key : Entity
{
    public Owner Owner { get; set; }
    public required string Pass { get; set; }
    
    public required string Name { get; set; }

    public List<Role> Roles { get; } = new();

    public static Key Create(Owner owner, string name)
    {
        var key = CreateEntity<Key>();
        key.Name = name;
        key.Pass = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
        key.Owner = owner;
        return key;
    }
}