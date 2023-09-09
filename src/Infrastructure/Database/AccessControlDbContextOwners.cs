using Core.Entities;
using Core.Entities.Owners;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Owner> Owners { get; set; }
    
	private void ConfigureOwnerEntities(ModelBuilder mb)
	{
		mb.Entity<Owner>().Property(o => o.Name).HasMaxLength(255);
	}
}