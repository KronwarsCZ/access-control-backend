using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbKey
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
	public IList<DbRolesKeys> DbRolesKeys { get; set; }
}