using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class Lock
{
    public Guid Id { get; set; }
    public string LockOwner { get; set; }
}

public class LockDbContext : DbContext
{
    public LockDbContext(DbContextOptions<LockDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lock> Lock { get; set; }
}