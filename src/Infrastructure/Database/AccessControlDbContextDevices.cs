using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Device> Devices { get; set; }
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<Device>().Property(d => d.Name).IsRequired().HasMaxLength(255);
        mb.Entity<Device>().Property(d => d.Type).IsRequired();
        mb.Entity<Device>().HasOne(a => a.Owner).WithMany(o => o.Devices).HasForeignKey(a => a.OwnerId);
    }
}