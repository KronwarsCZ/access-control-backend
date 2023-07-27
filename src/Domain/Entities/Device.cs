namespace Core.Entities;

public class Device : Entity
{
    public required Owner Owner { get; init; }
    
    public required string Name { get; set; }
    
    public required DeviceType Type { get; set; }
}
