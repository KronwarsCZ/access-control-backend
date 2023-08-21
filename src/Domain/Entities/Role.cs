using Framework.Identification;

namespace Core.Entities;

public class Role : Entity
{
    public required string Name { get; set; }
    
    /*public required List<Device> Devices { get; set; }

	public required List<Key> Keys { get; set; }

	public required List<Building> Buildings { get; set; }*/
    
    public static Role Create(string name)
    {
        var timestamp = DateTime.UtcNow;
        return new Role
        {
            Id = SequentialGuid.Create(),
            Name = name,
            CreatedUtc = timestamp,
            UpdatedUtc = timestamp,
            IsDeleted = false
        };
    }
}