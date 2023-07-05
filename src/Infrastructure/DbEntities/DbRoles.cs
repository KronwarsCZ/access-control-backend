using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbRoles
{
	[Key]
	[Required]
	public int Id { get; set; }
	
	[Required]
	public Guid Guid { get; set; }
	
	[Required]
	public string Name { get; set; }
	
	public int KeysFId { get; set; }
}