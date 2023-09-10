using Core.Entities.JoinEntities;
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
        
                
        // setup m to n, role <---> key
        mb.Entity<Key>().HasMany(r => r.Roles)
            .WithMany(d => d.Keys)
            .UsingEntity<RoleKey>(
                j => j
                    .HasOne(dr => dr.Role)
                    .WithMany()
                    .HasForeignKey(dr => dr.RoleId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(dr => dr.Key)
                    .WithMany()
                    .HasForeignKey(dr => dr.KeyId)
                    .OnDelete(DeleteBehavior.NoAction)
            );
    }
}