using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


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

        ConfigureBaseEntities(mb);

        ConfigureDeviceEntities(mb);
        ConfigureRoleEntities(mb);
        ConfigureKeyEntities(mb);
        ConfigureOwnerEntities(mb);
        ConfigureAreaEntities(mb);
        ConfigureBudildingEntities(mb);
        ConfigureAccountEntities(mb);
    }

    public void DetectChanges()
    {
        ChangeTracker.DetectChanges();
    }

    public void Add(IEnumerable<object> entities)
    {
        UpdateState(entities, _ => EntityState.Added);
    }

    public void Attach(IEnumerable<object> entities)
    {
        UpdateState(entities, _ => EntityState.Unchanged);
    }

    public void Detach(IEnumerable<object> entities)
    {
        UpdateState(entities, _ => EntityState.Detached);
    }

    public void DetachEntries<TEntity>()
        where TEntity : class
    {
        ChangeTracker.Entries().Where(x => x.Entity.GetType() == typeof(TEntity)).ToList().ForEach(e => e.State = EntityState.Detached);
    }

    public void Delete(IEnumerable<object> entities)
    {
        DetectChanges();
        UpdateState(entities, e => (e.State == EntityState.Added) switch {
            true => EntityState.Detached,
            false => EntityState.Deleted
        });
    }

    private void UpdateState(IEnumerable<object> entities, Func<EntityEntry, EntityState> state)
    {
        foreach (var entity in new List<object>(entities))
        {
            var entry = Entry(entity);
            entry.State = state(entry);
        }
    }
}
