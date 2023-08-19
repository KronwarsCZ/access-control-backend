using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Framework.Identification;
using Infrastructure.Mapping;

namespace Infrastructure.DbEntities;


public abstract class DbEntity : IIdentifiable
{
    protected DbEntity()
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
    
    public required DateTime CreatedUtc { get; init; }

    public required DateTime UpdatedUtc { get; set; }
    
    public bool IsDeleted { get; set; }
}

public interface IIdentifiable
{
    [Key]
    [Required]
    public Guid Id { get; set; }
}
