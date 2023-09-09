using Core.Entities;
using Core.Entities.Keys;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Key> Keys { get; set; }
    
	private void ConfigureKeyEntities(ModelBuilder mb)
	{
		mb.Entity<Key>().Property(k => k.Name).IsRequired().HasMaxLength(255);
		mb.Entity<Key>().Property(k => k.Pass).IsRequired().HasMaxLength(255);
	}
}