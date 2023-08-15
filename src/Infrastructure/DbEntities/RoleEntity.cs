namespace Infrastructure.DbEntities;

public class RoleEntity : DbEntity
{
    public required string Name { get; set; }

    // A Role is assigned to these Devices
    public List<DeviceEntity> Devices { get; }  = new();
	public List<JointDevicesRoles> DbDevicesRoles { get; }  = new();
	
	// A Role is assigned to these Keys
	public List<DbRolesKeys> DbRolesKeys { get; set; }
	
	// A Role can be accessed in these Buildings
	public List<DbBudildingRoles> DbBudildingRoles { get; set; }
}