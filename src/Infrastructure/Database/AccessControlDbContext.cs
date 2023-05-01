using Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Database;
public sealed partial class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Lock>().Property(a => a.PRIDANYFIELD).HasMaxLength(255);
        mb.Entity<Lock>().Property(b => b.Id).IsRequired();
    }
}