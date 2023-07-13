using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<BuildingEntity> Buildings { get; set; }
    
	private void ConfigureBudildingEntities(ModelBuilder mb)
	{
		mb.Entity<BuildingEntity>().Property(b => b.Id).IsRequired();
		mb.Entity<BuildingEntity>().Property(b => b.Name).IsRequired().HasMaxLength(255);
		mb.Entity<BuildingEntity>().Property(b => b.Address).IsRequired().HasMaxLength(255);
	}
}