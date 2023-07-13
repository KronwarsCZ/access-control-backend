using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class KeyEntity
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Pass { get; set; }
	
	// A key is has these Roles
	public List<JoinRolesKeys> DbRolesKeys { get; set; }
}