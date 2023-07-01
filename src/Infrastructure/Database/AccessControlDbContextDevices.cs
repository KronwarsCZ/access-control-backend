using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<DeviceEntity> Devices { get; } 
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<DeviceEntity>().Property(d => d.Id).IsRequired();
        mb.Entity<DeviceEntity>().Property(d => d.Owner).HasMaxLength(255).IsRequired();
    }
}