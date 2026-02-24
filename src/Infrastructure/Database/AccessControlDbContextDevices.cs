using Core.Entities.Devices;
using Core.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Device> Devices { get; set; }
    
    private void ConfigureDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<Device>().Property(d => d.Name).IsRequired().HasMaxLength(255);
        mb.Entity<Device>().Property(d => d.Type).IsRequired();
        
        // Hardware ID for provisioned devices (nullable, unique when set)
        mb.Entity<Device>().Property(d => d.HardwareId).HasMaxLength(20);
        mb.Entity<Device>().HasIndex(d => d.HardwareId)
            .IsUnique()
            .HasFilter("[HardwareId] IS NOT NULL AND [IsDeleted] = 0");
        
        // setup m to n, device <---> role
        mb.Entity<Device>().HasMany(d => d.Roles)
            .WithMany(r => r.Devices)
            .UsingEntity<DeviceRole>(
                j => j
                    .HasOne(dr => dr.Role)
                    .WithMany()
                    .HasForeignKey(dr => dr.RoleId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(dr => dr.Device)
                    .WithMany()
                    .HasForeignKey(dr => dr.DeviceId)
                    .OnDelete(DeleteBehavior.NoAction)
            );
    }
}