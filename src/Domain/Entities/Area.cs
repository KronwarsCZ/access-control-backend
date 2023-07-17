using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Area : Entity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; init; }

    [Required]
    [MaxLength(255)] public string Location { get; init; }
    
    public Owner Owner { get; init; }

    public List<Building> Buildings { get; init; }
}
