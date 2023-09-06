using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    /*
	public DbSet<DbDevicesRoles> DevicesRoles { get; set; }
	public DbSet<DbRolesKeys> RolesKeys { get; set; }

	private void ConfigureJointTables(ModelBuilder mb)
	{

		#region DevicesRoles

		// Setup composite keys
		mb.Entity<DbDevicesRoles>().HasKey(dr => new { dr.DeviceFId, dr.RoleFId });

		
		// Device and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<DeviceEntity>(dr => dr.DeviceEntity)
			.WithMany(d => d.DevicesRolesEntity)
			.HasForeignKey(dr => dr.DeviceFId)
            .OnDelete(DeleteBehavior.NoAction);
		// Role and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<RoleEntity>(dr => dr.RoleEntity)
			.WithMany(d => d.DbDevicesRoles)
			.HasForeignKey(dr => dr.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);

		#endregion

		#region RolesKeys
		
		// Setup composite keys
		mb.Entity<DbRolesKeys>().HasKey(rk => new { rk.RoleFId, rk.KeyFId });

		// Role and joining table have mismatching Id names
		mb.Entity<DbRolesKeys>()
			.HasOne<RoleEntity>(rk => rk.RoleEntity)
			.WithMany(r => r.DbRolesKeys)
			.HasForeignKey(rk => rk.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);
        
		// Key and joining table have mismatching Id names
		mb.Entity<DbRolesKeys>()
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
			.WithMany(b => b.BuildingRolesEntity)
			.HasForeignKey(br => br.BuildingFId)
            .OnDelete(DeleteBehavior.NoAction);
		// Role and joining table have mismatching Id names
		mb.Entity<DbBudildingRoles>()
			.HasOne<RoleEntity>(br => br.RoleEntity)
			.WithMany(r => r.DbBudildingRoles)
			.HasForeignKey(br => br.RoleFId)
            .OnDelete(DeleteBehavior.NoAction);

		#endregion
		
	}
	*/
}