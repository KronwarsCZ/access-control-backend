using Core.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    private void ConfigureBaseEntities(ModelBuilder mb)
    {
        mb.Entity<Entity>().UseTpcMappingStrategy();
        
        mb.Entity<Entity>().HasKey(e => e.Id);
        mb.Entity<Entity>().Property(e => e.CreatedUtc).IsRequired();
        mb.Entity<Entity>().Property(e => e.UpdatedUtc);
        mb.Entity<Entity>().Property(e => e.DeletedUtc);
        mb.Entity<Entity>().Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}
