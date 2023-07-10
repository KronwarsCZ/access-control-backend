using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbArea> Areas { get; set; }
    
	private void ConfigureAreaEntities(ModelBuilder mb)
	{
		mb.Entity<DbArea>().Property(a => a.Id).IsRequired();
		mb.Entity<DbArea>().Property(a => a.Name).HasMaxLength(255).IsRequired();
		mb.Entity<DbArea>().Property(a => a.Location).HasMaxLength(255).IsRequired();
	}
}