using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbRole> Roles { get; set; }
    
	private void ConfigureRoleEntities(ModelBuilder mb)
	{
		mb.Entity<DbRole>().Property(r => r.Id).IsRequired();
		mb.Entity<DbRole>().Property(r => r.Name).HasMaxLength(255).IsRequired();
	}
}