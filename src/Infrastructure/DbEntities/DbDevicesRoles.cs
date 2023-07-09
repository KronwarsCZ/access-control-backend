namespace Infrastructure.DbEntities;

public class DbDevicesRoles
{
	// JOINING TABLE Device <-> Role 
	public Guid DeviceFId { get; set; }
	public DbDevice DbDevice { get; set; }
	
	public Guid RoleFId { get; set; }
	public DbRole DbRole { get; set; }
}