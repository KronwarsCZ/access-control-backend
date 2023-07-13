using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<RoleEntity> Roles { get; set; }
    
	private void ConfigureRoleEntities(ModelBuilder mb)
	{
		mb.Entity<RoleEntity>().Property(r => r.Id).IsRequired();
		mb.Entity<RoleEntity>().Property(r => r.Name).IsRequired().HasMaxLength(255);
	}
}