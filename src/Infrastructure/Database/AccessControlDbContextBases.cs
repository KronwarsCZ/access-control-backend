using Core.Entities.BaseEntities;
using Core.Entities.Owners;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    private void ConfigureBaseOwnedEntities(ModelBuilder mb)
    {
        mb.Entity<OwnedEntity>().UseTpcMappingStrategy();
        
        mb.Entity<OwnedEntity>().HasKey(e => e.Id);
        mb.Entity<OwnedEntity>().Property(e => e.CreatedUtc).IsRequired();
        mb.Entity<OwnedEntity>().Property(e => e.UpdatedUtc);
        mb.Entity<OwnedEntity>().Property(e => e.DeletedUtc);
        mb.Entity<OwnedEntity>().Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        mb.Entity<OwnedEntity>().HasQueryFilter(e => !e.IsDeleted);
        
        // Configure the Owner relationship for all OwnedEntities
        mb.Entity<OwnedEntity>()
            .HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
