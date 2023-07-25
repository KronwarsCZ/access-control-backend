using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbDevicesRoles> DevicesRoles { get; set; }
	public DbSet<JoinRolesKeys> RolesKeys { get; set; }

	private void ConfigureJointTables(ModelBuilder mb)
	{

		#region DevicesRoles

		// Setup composite keys
		mb.Entity<DbDevicesRoles>().HasKey(dr => new { dr.DeviceFId, dr.RoleFId });

		
		// Device and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<DeviceEntity>(dr => dr.DbDevice)
			.WithMany(d => d.DbDevicesRoles)
			.HasForeignKey(dr => dr.DeviceFId)
            .OnDelete(DeleteBehavior.NoAction);
		// Role and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<RoleEntity>(dr => dr.DbRole)
			.WithMany(d => d.DbDevicesRoles)
			.HasForeignKey(dr => dr.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);

		#endregion

		#region RolesKeys
		
		// Setup composite keys
		mb.Entity<JoinRolesKeys>().HasKey(rk => new { rk.RoleFId, rk.KeyFId });

		// Role and joining table have mismatching Id names
		mb.Entity<JoinRolesKeys>()
			.HasOne<RoleEntity>(rk => rk.DbRole)
			.WithMany(r => r.DbRolesKeys)
			.HasForeignKey(rk => rk.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);
        
		// Key and joining table have mismatching Id names
		mb.Entity<JoinRolesKeys>()
			.HasOne<KeyEntity>(rk => rk.KeyEntity)
			.WithMany(r => r.DbRolesKeys)
			.HasForeignKey(rk => rk.KeyFId)
            .OnDelete(DeleteBehavior.NoAction);

		#endregion


		#region BuildingRoles
		
		// Setup composite keys
		mb.Entity<DbBudildingRoles>().HasKey(br => new { br.BuildingFId, br.RoleFId });

		// Building and joining table have mismatching Id names
		mb.Entity<DbBudildingRoles>()
			.HasOne<BuildingEntity>(br => br.BuildingEntity)
			.WithMany(b => b.DbBudildingRoles)
			.HasForeignKey(br => br.BuildingFId)
            .OnDelete(DeleteBehavior.NoAction);
		// Role and joining table have mismatching Id names
		mb.Entity<DbBudildingRoles>()
			.HasOne<RoleEntity>(br => br.DbRole)
			.WithMany(r => r.DbBudildingRoles)
			.HasForeignKey(br => br.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);

		#endregion
		
	}
}