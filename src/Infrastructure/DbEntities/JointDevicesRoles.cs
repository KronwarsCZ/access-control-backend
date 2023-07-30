namespace Infrastructure.DbEntities;

public class DbDevicesRoles
{
	// JOINING TABLE Device <-> Role 
	public Guid DeviceFId { get; set; }
	public DeviceEntity DeviceEntity { get; set; }
	
	public Guid RoleFId { get; set; }
	public RoleEntity RoleEntity { get; set; }
}