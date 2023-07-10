using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Owner
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	public ICollection<Area> Areas { get; set; }
}