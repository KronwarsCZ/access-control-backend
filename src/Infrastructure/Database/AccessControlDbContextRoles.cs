using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Role> Roles { get; set; }
    
	private void ConfigureRoleEntities(ModelBuilder mb)
	{
		mb.Entity<Role>().Property(r => r.Id).IsRequired();
		mb.Entity<Role>().Property(r => r.Name).IsRequired().HasMaxLength(255);
        
        // setup m to n, device <---> role
		mb.Entity<Role>().HasMany(r => r.Devices)
                        .WithMany(d => d.Roles);
	}
}