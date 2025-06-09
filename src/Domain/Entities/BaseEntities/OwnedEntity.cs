using System.ComponentModel.DataAnnotations;
using Core.Entities.Owners;
using Framework.Extensions;
using Framework.Identification;

namespace Core.Entities.BaseEntities;

public abstract class OwnedEntity : Entity
{
    protected OwnedEntity()
    {
        Id = SequentialGuid.Create();
    }
    
    public Owner Owner { get; set; }
    
    public Guid OwnerId { get; set; }
    
    protected static TEntity CreateEntity<TEntity>()
        where TEntity : OwnedEntity
    {
        var timestamp = DateTime.UtcNow;
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true);

        (entity as OwnedEntity).MapRef(e =>
        {
            e.CreatedUtc = timestamp;
            e.UpdatedUtc = timestamp;
            return e;
        });

        return entity;
    }
    
    public override Guid GetPrimaryId() => OwnerId;
}
