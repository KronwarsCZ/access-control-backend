using Core.Entities;
using Core.Entities.DeviceIoTInfo;
using Core.Entities.Devices;
using Core.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<DeviceIoTInfo> DeviceIoTInfo { get; set; }
    
    private void ConfigureDeviceIoTInfoEntities(ModelBuilder mb)
    {
        mb.Entity<DeviceIoTInfo>().Property(i => i.FirmwareVersion).IsRequired().HasMaxLength(255);
        mb.Entity<DeviceIoTInfo>().Property(i => i.IoTHubDeviceId).IsRequired();
        
        
        // setup 1 to optional 1, device <---> deviceIoTInfo
        mb.Entity<DeviceIoTInfo>()
            .HasOne(i => i.Device)
            .WithOne(d => d.IoTInfo)
            .HasForeignKey<DeviceIoTInfo>(i => i.DeviceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
