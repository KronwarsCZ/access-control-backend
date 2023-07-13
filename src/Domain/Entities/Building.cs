using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Building
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Address { get; set; }
	
	
	public Area Area { get; set; }
	
	public List<Role> Roles { get; set; }
}