using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Key : Entity
{ 
    [Required]
	[MaxLength(255)]
	public string Name { get; init; }
	
	[Required]
	[MaxLength(255)]
	public string Pass { get; init; }
	
	public List<Role> Roles { get; set; }
}