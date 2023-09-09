using System.ComponentModel.DataAnnotations;
using Framework.Identification;

namespace Core.Entities.BaseEntities;

public abstract class Entity : IIdentifiable
{
    protected Entity()
    {
        Id = SequentialGuid.Create();
    }

    public Guid Id { get; init; }
    public DateTime CreatedUtc { get; init; }

    public DateTime UpdatedUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    protected static TEntity CreateEntity<TEntity>()
        where TEntity : Entity
    {
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true);
        return entity;
    }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}

