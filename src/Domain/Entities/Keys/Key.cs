using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Core.Entities.Keys;

public class Key : OwnedEntity
{
    /// <summary>
    /// The RFID UID read from the physical keycard, stored as uppercase hex (e.g. "AABBCCDD").
    /// This is what lock devices compare against scanned cards.
    /// </summary>
    public required string KeyUid { get; set; }
    
    public DateTime ValidityStartUtc { get; set; }
    public DateTime? ValidityEndUtc { get; set; }

    public required string Name { get; set; }

    public List<Role> Roles { get; } = new();

    public static Key Create(Owner owner, string name, string keyUid, DateTime? validityStartUtc = null, DateTime? validityEndUtc = null)
    {
        var key = CreateEntity<Key>();
        key.Name = name;
        key.KeyUid = keyUid.Replace(" ", "").Replace("-", "").Replace(":", "").ToUpperInvariant();
        key.Owner = owner;
        key.OwnerId = owner.Id;
        key.ValidityStartUtc = validityStartUtc ?? key.CreatedUtc;
        key.ValidityEndUtc = validityEndUtc;
        return key;
    }
}