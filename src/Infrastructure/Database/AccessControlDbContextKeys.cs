using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<KeyEntity> Keys { get; set; }
    
	private void ConfigureKeyEntities(ModelBuilder mb)
	{
		mb.Entity<KeyEntity>().Property(k => k.Name).IsRequired().HasMaxLength(255);
		mb.Entity<KeyEntity>().Property(k => k.Pass).IsRequired().HasMaxLength(255);
	}
}