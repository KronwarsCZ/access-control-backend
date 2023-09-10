using Core.Entities.BaseEntities;
using Core.Entities.Roles;
using Framework.Identification;

namespace Core.Entities.Keys;

public class Key : Entity
{
    public required string Pass { get; set; }
    
    public required string Name { get; set; }

    public List<Role> Roles { get; } = new();

    public static Key Create(string name)
    {
        var key = CreateEntity<Key>();
        key.Name = name;
        key.Pass = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
        return key;
    }
}