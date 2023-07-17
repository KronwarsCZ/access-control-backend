using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public abstract class Entity : IIdentifiable
{
    public required Guid Id { get; init; }

    public required DateTime CreatedUtc { get; init; }

    public required DateTime UpdatedUtc { get; init; }

    public required bool IsDeleted { get; init; }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}
