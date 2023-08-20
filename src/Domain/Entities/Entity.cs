using System.ComponentModel.DataAnnotations;
using Framework.Identification;

namespace Core.Entities;

public abstract class Entity : IIdentifiable
{
    protected Entity()
    {
    }
    
    private Guid? id;

    /// <summary>
    /// Unique identifier of the entity.
    /// </summary>
    public virtual Guid Id
    {
        get
        {
            // The Id is assigned to entity lazily in order to reduce amount of new Guids. E.g. when the entity is retrieved from the database.
            // It is constructed and the Id is immediately assigned so it is unnecessary to generate a new Guid in that case.
            if (!id.HasValue)
            {
                id = SequentialGuid.Create();
            }
            return id.Value;
        }
        set => id = value;
    }
    
    public DateTime CreatedUtc { get; init; }

    public DateTime UpdatedUtc { get; set; }
    
    public bool IsDeleted { get; set; }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; set; }
}

