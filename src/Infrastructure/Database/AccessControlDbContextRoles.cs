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
	}
}