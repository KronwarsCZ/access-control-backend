namespace Core.Entities;

public class Device : Entity
{
    public required Owner Owner { get; init; }
    public required Guid OwnerId { get; init; }
    
    public required string Name { get; init; }
    
    public required DeviceType Type { get; init; }
}
