using System.ComponentModel.DataAnnotations;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Identification;

namespace Core.Entities.BaseEntities;

public abstract class Entity : IIdentifiable
{
    protected Entity()
    {
        Id = SequentialGuid.Create();
    }

    public Guid Id { get; init; }
    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public DateTime DeletedUtc { get; set; }
    
    public Owner Owner { get; set; }
    
    public Guid OwnerId { get; }
    
    public bool IsDeleted { get; set; }
    
    protected static TEntity CreateEntity<TEntity>()
        where TEntity : Entity
    {
        var timestamp = DateTime.UtcNow;
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true);

        (entity as Entity).MapRef(e =>
        {
            e.CreatedUtc = timestamp;
            e.UpdatedUtc = timestamp;
            return e;
        });

        return entity;
    }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}

