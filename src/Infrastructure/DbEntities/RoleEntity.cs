using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class RoleEntity
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	// A Role is assigned to these Devices
	public List<DbDevicesRoles> DbDevicesRoles { get; set; }
	
	// A Role is assigned to these Keys
	public List<JoinRolesKeys> DbRolesKeys { get; set; }
	
	// A Role can be accesed in these Buildings
	public List<DbBudildingRoles> DbBudildingRoles { get; set; }
}