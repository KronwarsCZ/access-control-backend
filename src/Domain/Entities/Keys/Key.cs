using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Core.Entities.Keys;

public class Key : OwnedEntity
{
    public required string Pass { get; set; }
    
    public DateTime ValidityStartUtc { get; set; }
    public DateTime? ValidityEndUtc { get; set; }

    public required string Name { get; set; }

    public List<Role> Roles { get; } = new();

    public static Key Create(Owner owner, string name, DateTime? validityStartUtc = null, DateTime? validityEndUtc = null)
    {
        var key = CreateEntity<Key>();
        key.Name = name;
        key.Pass = $"{Guid.NewGuid()}-{Guid.NewGuid()}"; // generate the secret token
        key.Owner = owner;
        key.OwnerId = owner.Id;
        key.ValidityStartUtc = validityStartUtc ?? key.CreatedUtc;
        key.ValidityEndUtc = validityEndUtc;
        return key;
    }
}