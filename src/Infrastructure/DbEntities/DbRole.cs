using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbRole
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	// A Role is assigned to these Devices
	public ICollection<DbDevicesRoles> DbDevicesRoles { get; set; }
	
	// A Role is assigned to these Keys
	public ICollection<DbRolesKeys> DbRolesKeys { get; set; }
	
	// A Role can be accesed in these Buildings
	public ICollection<DbBudildingRoles> DbBudildingRoles { get; set; }
}