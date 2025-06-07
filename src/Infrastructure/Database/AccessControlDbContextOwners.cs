using Core.Entities.Owners;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<Owner> Owners { get; set; }
    
	private void ConfigureOwnerEntities(ModelBuilder mb)
    {
        mb.Entity<Owner>().UseTpcMappingStrategy();
        mb.Entity<Owner>().HasKey(o => o.Id);
		mb.Entity<Owner>().Property(o => o.Name).HasMaxLength(255);
	}
}