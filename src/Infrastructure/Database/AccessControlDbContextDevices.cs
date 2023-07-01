using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<DbDevice> Devices { get; set; }
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<DbDevice>().Property(d => d.Id).IsRequired();
        mb.Entity<DbDevice>().Property(d => d.Owner).HasMaxLength(255).IsRequired();
    }
}