namespace Core.Entities;

public class Key : Entity
{
    public required string Pass { get; init; }
    
    public required string Name { get; set; }

}