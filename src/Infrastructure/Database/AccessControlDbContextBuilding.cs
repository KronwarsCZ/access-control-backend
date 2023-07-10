using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbBuilding> Buildings { get; set; }
    
	private void ConfigureBudildingEntities(ModelBuilder mb)
	{
		mb.Entity<DbBuilding>().Property(b => b.Id).IsRequired();
		mb.Entity<DbBuilding>().Property(b => b.Name).HasMaxLength(255).IsRequired();
		mb.Entity<DbBuilding>().Property(b => b.Address).HasMaxLength(255).IsRequired();
	}
}