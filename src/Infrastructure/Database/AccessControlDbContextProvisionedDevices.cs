using Core.Entities.DeviceProvisioning;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<ProvisionedDevice> ProvisionedDevices { get; set; }

    private void ConfigureProvisionedDeviceEntities(ModelBuilder mb)
    {
        mb.Entity<ProvisionedDevice>().HasKey(p => p.Id);

        mb.Entity<ProvisionedDevice>().Property(p => p.DeviceId)
            .IsRequired()
            .HasMaxLength(20); // DEV-XXXXXXXXXXXX = 16 chars, allow some buffer

        mb.Entity<ProvisionedDevice>().HasIndex(p => p.DeviceId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0"); // Unique only among non-deleted

        mb.Entity<ProvisionedDevice>().Property(p => p.CertificateSerialNumber)
            .IsRequired()
            .HasMaxLength(64);

        mb.Entity<ProvisionedDevice>().Property(p => p.CertificateThumbprint)
            .IsRequired()
            .HasMaxLength(64);

        mb.Entity<ProvisionedDevice>().HasIndex(p => p.CertificateThumbprint);

        mb.Entity<ProvisionedDevice>().Property(p => p.CertificateIssuedUtc)
            .IsRequired();

        mb.Entity<ProvisionedDevice>().Property(p => p.CertificateExpiresUtc)
            .IsRequired();

        mb.Entity<ProvisionedDevice>().Property(p => p.IsActive)
            .HasDefaultValue(true);

        mb.Entity<ProvisionedDevice>().Property(p => p.RenewalCount)
            .HasDefaultValue(0);

        // Optional relationship to Device entity
        mb.Entity<ProvisionedDevice>()
            .HasOne(p => p.LinkedDevice)
            .WithMany()
            .HasForeignKey(p => p.LinkedDeviceId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
