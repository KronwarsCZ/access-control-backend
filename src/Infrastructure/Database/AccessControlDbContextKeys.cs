using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbKey> Keys { get; set; }
    
	private void ConfigureKeyEntities(ModelBuilder mb)
	{
		mb.Entity<DbKey>().Property(k => k.Id).IsRequired();
		mb.Entity<DbKey>().Property(k => k.Name).HasMaxLength(255).IsRequired();
		mb.Entity<DbKey>().Property(k => k.Pass).HasMaxLength(255).IsRequired();
	}
}