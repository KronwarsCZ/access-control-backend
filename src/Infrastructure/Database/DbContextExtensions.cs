using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Database;

public static class DbContextExtensions
{
    public static EntityEntry<TEnt> AddIfNotExists<TEnt>(this DbSet<TEnt> dbSet, TEnt entity) where TEnt : class
    {
        var exists = dbSet.Any(c => entity == c);
        return exists
            ? null
            : dbSet.Add(entity);
    }
}
