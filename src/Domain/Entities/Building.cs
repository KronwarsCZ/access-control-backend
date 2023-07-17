using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Building : Entity
{
    [Required]
	[MaxLength(255)]
	public string Name { get; init; }
	
	[Required]
	[MaxLength(255)]
	public string Address { get; init; }
	
	
	public Area Area { get; set; }
	
	public List<Role> Roles { get; set; }
}