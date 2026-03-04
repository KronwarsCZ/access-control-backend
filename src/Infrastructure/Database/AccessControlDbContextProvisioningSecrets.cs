using Core.Entities.DeviceProvisioning;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<ProvisioningSecret> ProvisioningSecrets { get; set; }

    private void ConfigureProvisioningSecretEntities(ModelBuilder mb)
    {
        mb.Entity<ProvisioningSecret>().HasKey(s => s.Id);

        mb.Entity<ProvisioningSecret>().Property(s => s.SecretHash)
            .IsRequired()
            .HasMaxLength(64);

        mb.Entity<ProvisioningSecret>().Property(s => s.Label)
            .HasMaxLength(128);

        mb.Entity<ProvisioningSecret>().Property(s => s.IsActive)
            .HasDefaultValue(true);

        mb.Entity<ProvisioningSecret>().HasIndex(s => s.SecretHash)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        mb.Entity<ProvisioningSecret>().HasIndex(s => s.OwnerId)
            .HasFilter("[IsDeleted] = 0");

        mb.Entity<ProvisioningSecret>()
            .HasOne(s => s.Owner)
            .WithMany()
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
