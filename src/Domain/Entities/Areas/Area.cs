using Core.Entities.BaseEntities;
using Core.Entities.Buildings;
using Core.Entities.Owners;

namespace Core.Entities.Areas;

public class Area : Entity
{
    public required string Name { get; set; }

    public required string Location { get; init; }
    
    public required Owner Owner { get; init; }

    public required List<Building> Buildings { get; init; }
}
