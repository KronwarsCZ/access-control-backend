using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<OwnerEntity> Owners { get; set; }
    
	private void ConfigureOwnerEntities(ModelBuilder mb)
	{
		mb.Entity<OwnerEntity>().Property(o => o.Id).IsRequired();
		mb.Entity<OwnerEntity>().Property(r => r.Name).IsRequired().HasMaxLength(255);
	}
}