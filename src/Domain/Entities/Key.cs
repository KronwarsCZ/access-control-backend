using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Key
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
	
	public List<Role> Roles { get; set; }
}