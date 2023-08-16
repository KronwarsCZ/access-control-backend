using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<DeviceEntity> Devices { get; set; }
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<DeviceEntity>().Property(d => d.Name).IsRequired().HasMaxLength(255);
        mb.Entity<DeviceEntity>().Property(d => d.Type).HasConversion(new EnumToStringConverter<DeviceTypeEntity>());
        mb.Entity<DeviceEntity>().HasOne(a => a.Owner).WithMany(o => o.Devices).HasForeignKey(a => a.OwnerId);
    }
}