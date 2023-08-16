namespace Infrastructure.DbEntities;

public class RoleEntity : DbEntity
{
    public required string Name { get; set; }

    // A Role is assigned to these Devices
    public List<DeviceEntity> Devices { get; } = new();
	public List<JointDevicesRoles> DbDevicesRoles { get; }  = new();
	
	// A Role is assigned to these Keys
    public List<KeyEntity> Keys { get; } = new();
	public List<JointRolesKeys> DbRolesKeys { get; } = new();
	
	// A Role can be accessed in these Buildings
    public List<BuildingEntity> Buildings { get; } = new();
	public List<JointBudildingRoles> DbBudildingRoles { get; } = new();
}