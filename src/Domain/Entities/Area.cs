namespace Core.Entities;

public class Area : Entity
{
    public required string Location { get; init; }
    
    public required Owner Owner { get; init; }

    public required List<Building> Buildings { get; init; }
}
