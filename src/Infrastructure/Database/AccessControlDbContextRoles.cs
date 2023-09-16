using Core.Entities.JoinEntities;
using Core.Entities.Roles;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Role> Roles { get; set; }
    
	private void ConfigureRoleEntities(ModelBuilder mb)
	{
		mb.Entity<Role>().Property(r => r.Name).IsRequired().HasMaxLength(255);

        // setup m to n, device <---> role
        mb.Entity<Role>().HasMany(r => r.Devices)
            .WithMany(d => d.Roles)
            .UsingEntity<DeviceRole>(
                j => j
                    .HasOne(dr => dr.Device)
                    .WithMany()
                    .HasForeignKey(dr => dr.DeviceId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(dr => dr.Role)
                    .WithMany()
                    .HasForeignKey(dr => dr.RoleId)
                    .OnDelete(DeleteBehavior.NoAction)
        );
        
        // setup m to n, role <---> key
        mb.Entity<Role>().HasMany(r => r.Keys)
            .WithMany(d => d.Roles)
            .UsingEntity<RoleKey>(
                j => j
                    .HasOne(dr => dr.Key)
                    .WithMany()
                    .HasForeignKey(dr => dr.KeyId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(dr => dr.Role)
                    .WithMany()
                    .HasForeignKey(dr => dr.RoleId)
                    .OnDelete(DeleteBehavior.NoAction)
            );
    }
}