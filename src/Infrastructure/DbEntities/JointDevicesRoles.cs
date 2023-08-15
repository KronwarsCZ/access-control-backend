namespace Infrastructure.DbEntities;

public class JointDevicesRoles : DbJointEntity
{
	// JOINING TABLE Device <-> Role 
	public Guid DeviceId { get; set; }
	public DeviceEntity DeviceEntity { get; set; } = null!;
	
	public Guid RoleId { get; set; }
	public RoleEntity RoleEntity { get; set; } = null!;
}