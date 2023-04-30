using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;
public class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options)
        : base(options)
    {
    }

    // Define your entity DbSet properties here
    public DbSet<AccessControlDbContext> MyEntities { get; set; }

    // Override OnModelCreating method if you need to configure entity relationships, etc.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships, etc. here
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionString");
    }
}