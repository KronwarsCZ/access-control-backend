namespace Infrastructure.DbEntities;

public class DbDevicesRoles
{
	// JOINING TABLE Device <-> Role 
	public Guid DeviceFId { get; set; }
	public DeviceEntity DbDevice { get; set; }
	
	public Guid RoleFId { get; set; }
	public RoleEntity DbRole { get; set; }
}