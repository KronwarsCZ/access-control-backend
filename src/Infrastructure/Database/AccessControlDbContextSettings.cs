using Core.Entities.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<SystemSettings> SystemSettings { get; set; }

    private void ConfigureSystemSettingsEntities(ModelBuilder mb)
    {
        mb.Entity<SystemSettings>().HasKey(s => s.Id);

        mb.Entity<SystemSettings>().Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(128);

        mb.Entity<SystemSettings>().HasIndex(s => s.Key)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        mb.Entity<SystemSettings>().Property(s => s.Value)
            .HasMaxLength(4000);

        mb.Entity<SystemSettings>().Property(s => s.Description)
            .HasMaxLength(512);
    }
}
