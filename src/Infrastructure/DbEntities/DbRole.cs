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
	public IList<DbDevicesRoles> DbDevicesRoles { get; set; }
	
	// A Role is assigned to these Keys
	public IList<DbRolesKeys> DbRolesKeys { get; set; }
}