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

        ConfigureBaseEntities(mb);

        ConfigureDeviceEntities(mb);
        ConfigureRoleEntities(mb);
        ConfigureKeyEntities(mb);
        ConfigureOwnerEntities(mb);
        ConfigureAreaEntities(mb);
        ConfigureBudildingEntities(mb);
        
        ConfigureJointTables(mb);
    }
}
