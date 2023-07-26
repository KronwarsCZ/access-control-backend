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
        mb.Entity<DeviceEntity>().Property(d => d.DeviceType).HasConversion(new EnumToStringConverter<DeviceType>());
    }
}