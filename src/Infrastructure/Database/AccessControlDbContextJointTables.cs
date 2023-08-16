using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<JointDevicesRoles> DevicesRoles { get; set; }
	public DbSet<JointRolesKeys> RolesKeys { get; set; }
    
    public DbSet<JointBudildingRoles> BuildingRoles { get; set; }

	private void ConfigureJointTables(ModelBuilder mb)
	{

		#region DevicesRoles

        mb.Entity<DeviceEntity>()
            .HasMany(e => e.Roles)
            .WithMany(e => e.Devices)
            .UsingEntity<JointDevicesRoles>(
                r => r.HasOne(e => e.RoleEntity).WithMany(e => e.DbDevicesRoles).HasForeignKey(e => e.RoleId),
                l => l.HasOne(e => e.DeviceEntity).WithMany(e => e.DevicesRolesEntity).HasForeignKey(e => e.DeviceId),
                j => j.Property(e => e.CreatedUtc).HasDefaultValueSql("GETUTCDATE()"));

        #endregion

		#region RolesKeys

        mb.Entity<RoleEntity>()
            .HasMany(e => e.Keys)
            .WithMany(e => e.Roles)
            .UsingEntity<JointRolesKeys>(
                r => r.HasOne(e => e.KeyEntity).WithMany(e => e.DbRolesKeys).HasForeignKey(e => e.KeyId),
                l => l.HasOne(e => e.RoleEntity).WithMany(e => e.DbRolesKeys).HasForeignKey(e => e.RoleId),
                j => j.Property(e => e.CreatedUtc).HasDefaultValueSql("GETUTCDATE()"));

		#endregion


		#region BuildingRoles

        mb.Entity<BuildingEntity>()
            .HasMany(e => e.Roles)
            .WithMany(e => e.Buildings)
            .UsingEntity<JointBudildingRoles>(
                r => r.HasOne(e => e.RoleEntity).WithMany(e => e.DbBudildingRoles).HasForeignKey(e => e.RoleId),
                l => l.HasOne(e => e.BuildingEntity).WithMany(e => e.DbBuildingRoles).HasForeignKey(e => e.BuildingId),
                j => j.Property(e => e.CreatedUtc).HasDefaultValueSql("GETUTCDATE()"));

        #endregion
    }
}