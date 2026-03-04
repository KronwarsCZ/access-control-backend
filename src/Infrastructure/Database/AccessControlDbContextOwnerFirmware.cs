using Core.Entities.Ota;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<OwnerFirmware> OwnerFirmwares { get; set; }

    private void ConfigureOwnerFirmwareEntities(ModelBuilder mb)
    {
        mb.Entity<OwnerFirmware>().HasKey(f => f.Id);

        mb.Entity<OwnerFirmware>().Property(f => f.FirmwareVersion)
            .IsRequired()
            .HasMaxLength(31);

        mb.Entity<OwnerFirmware>().Property(f => f.IsEnabled)
            .HasDefaultValue(false);

        mb.Entity<OwnerFirmware>().HasIndex(f => f.OwnerId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        mb.Entity<OwnerFirmware>()
            .HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
