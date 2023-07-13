using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<AreaEntity> Areas { get; set; }
    
	private void ConfigureAreaEntities(ModelBuilder mb)
	{
		mb.Entity<AreaEntity>().Property(a => a.Id).IsRequired();
		mb.Entity<AreaEntity>().Property(a => a.Name).HasMaxLength(255).IsRequired();
		mb.Entity<AreaEntity>().Property(a => a.Location).HasMaxLength(255).IsRequired();
	}
}