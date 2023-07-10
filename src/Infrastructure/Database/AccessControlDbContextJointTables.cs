using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
	public DbSet<DbDevicesRoles> DevicesRoles { get; set; }
	public DbSet<DbRolesKeys> RolesKeys { get; set; }

	private void ConfigureJointTables(ModelBuilder mb)
	{

		#region DevicesRoles

		// Setup composite keys
		mb.Entity<DbDevicesRoles>().HasKey(dr => new { dr.DeviceFId, dr.RoleFId });

		
		// Device and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<DbDevice>(dr => dr.DbDevice)
			.WithMany(d => d.DbDevicesRoles)
			.HasForeignKey(dr => dr.DeviceFId);
		// Role and joining table have mismatching Id names
		mb.Entity<DbDevicesRoles>()
			.HasOne<DbRole>(dr => dr.DbRole)
			.WithMany(d => d.DbDevicesRoles)
			.HasForeignKey(dr => dr.RoleFId);

		#endregion

		#region RolesKeys
		
		// Setup composite keys
		mb.Entity<DbRolesKeys>().HasKey(rk => new { rk.RoleFId, rk.KeyFId });

		// Role and joining table have mismatching Id names
		mb.Entity<DbRolesKeys>()
			.HasOne<DbRole>(rk => rk.DbRole)
			.WithMany(r => r.DbRolesKeys)
			.HasForeignKey(rk => rk.RoleFId);
		// Key and joining table have mismatching Id names
		mb.Entity<DbRolesKeys>()
			.HasOne<DbKey>(rk => rk.DbKey)
			.WithMany(r => r.DbRolesKeys)
			.HasForeignKey(rk => rk.KeyFId);

		#endregion


		#region BuildingRoles
		
		// Setup composite keys
		mb.Entity<DbBudildingRoles>().HasKey(br => new { br.BuildingFId, br.RoleFId });

		// Building and joining table have mismatching Id names
		mb.Entity<DbBudildingRoles>()
			.HasOne<DbBuilding>(br => br.DbBuilding)
			.WithMany(b => b.DbBudildingRoles)
			.HasForeignKey(br => br.BuildingFId);
		// Role and joining table have mismatching Id names
		mb.Entity<DbBudildingRoles>()
			.HasOne<DbRole>(br => br.DbRole)
			.WithMany(r => r.DbBudildingRoles)
			.HasForeignKey(br => br.RoleFId);

		#endregion
		
	}
}