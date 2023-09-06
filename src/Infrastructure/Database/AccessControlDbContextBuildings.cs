using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Building> Buildings { get; set; }
    
	private void ConfigureBudildingEntities(ModelBuilder mb)
	{
		mb.Entity<Building>().Property(b => b.Name).IsRequired().HasMaxLength(255);
		mb.Entity<Building>().Property(b => b.Address).IsRequired().HasMaxLength(255);
	}
}