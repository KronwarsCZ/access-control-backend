using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    private void ConfigureBaseEntities(ModelBuilder mb)
    {
        mb.Entity<DbEntity>().UseTpcMappingStrategy();
        mb.Entity<DbEntity>().HasKey(e => e.Id);
        mb.Entity<DbEntity>().Property(e => e.CreatedUtc).IsRequired();
        mb.Entity<DbEntity>().Property(e => e.UpdatedUtc);
        mb.Entity<DbEntity>().Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}
