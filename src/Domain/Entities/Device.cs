using Framework.Identification;

namespace Core.Entities;

public class Device : Entity
{
    public Owner Owner { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public required DeviceType Type { get; set; }

    public List<Role> Roles { get; set; }
    
    public static Device Create(string name)
    {
        var timestamp = DateTime.UtcNow;
        return new Device
        {
            Id = SequentialGuid.Create(),
            Name = name,
            Type = DeviceType.Lock,
            IsDeleted = false,
            CreatedUtc = timestamp,
            UpdatedUtc = timestamp
        };
    }
}
