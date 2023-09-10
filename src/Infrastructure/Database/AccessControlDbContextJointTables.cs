using Core.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<DeviceRole> DeviceRole { get; set; }

    private void ConfigureJoinEntities(ModelBuilder mb)
    {
        mb.Entity<DeviceRole>().HasKey(dr => new { dr.DeviceId, dr.RoleId });
    }
}