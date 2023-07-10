using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbOwner> Owners { get; set; }
    
	private void ConfigureOwnerEntities(ModelBuilder mb)
	{
		mb.Entity<DbOwner>().Property(o => o.Id).IsRequired();
		mb.Entity<DbOwner>().Property(r => r.Name).HasMaxLength(255).IsRequired();
	}
}