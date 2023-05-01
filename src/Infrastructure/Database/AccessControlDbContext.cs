using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;
public class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Lock> Lock { get; set; }
    public DbSet<Person> Person { get; set; }

}