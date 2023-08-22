using Framework.Identification;

namespace Core.Entities;

public class Device : Entity
{
    public required Owner Owner { get; init; }
    public required Guid OwnerId { get; init; }
    public required string Name { get; init; }
    public required DeviceType Type { get; init; }

    public static Device Create(Owner owner, string name)
    {
        var timestamp = DateTime.UtcNow;
        return new Device
        {
            Id = SequentialGuid.Create(),
            Name = name,
            Type = DeviceType.Lock,
            Owner = owner,
            OwnerId = owner.Id,
            IsDeleted = false,
            CreatedUtc = timestamp,
            UpdatedUtc = timestamp
        };
    }
}
