using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public abstract class Entity : IIdentifiable
{
    public Guid Id { get; init; }

    public required DateTime CreatedUtc { get; init; }

    public required DateTime UpdatedUtc { get; set; }

    public required string Name { get; set; }
    
    public bool IsDeleted { get; set; }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}
