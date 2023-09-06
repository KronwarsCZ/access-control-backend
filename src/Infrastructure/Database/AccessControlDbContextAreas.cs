using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Area> Areas { get; set; }
    
	private void ConfigureAreaEntities(ModelBuilder mb)
	{
		mb.Entity<Area>().Property(a => a.Name).IsRequired().HasMaxLength(255);
		mb.Entity<Area>().Property(a => a.Location).IsRequired().HasMaxLength(255);
	}
}