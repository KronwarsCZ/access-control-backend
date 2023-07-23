namespace Infrastructure.DbEntities;

public class RoleEntity : DbEntity
{
    // A Role is assigned to these Devices
	public List<DbDevicesRoles> DbDevicesRoles { get; set; }
	
	// A Role is assigned to these Keys
	public List<JoinRolesKeys> DbRolesKeys { get; set; }
	
	// A Role can be accessed in these Buildings
	public List<DbBudildingRoles> DbBudildingRoles { get; set; }
}