using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Device> Devices { get; set; }
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<Device>().Property(b => b.Owner).HasMaxLength(255).IsRequired();
    }
}