using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Infrastructure.Mapping;

namespace Infrastructure.DbEntities;

public abstract class DbEntity : IIdentifiable
{
    public Guid Id { get; init; }

    public required DateTime CreatedUtc { get; init; }

    public required DateTime UpdatedUtc { get; set; }
    
    public bool IsDeleted { get; set; }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}
