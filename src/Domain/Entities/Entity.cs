using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Entity
{
    [Key]
    [Required]
    public Guid Id { get; init; }

    public DateTime CreatedUtc { get; init; }

    public DateTime UpdateUtc { get; init; }

    public bool IsDeleted { get; init; }
}
