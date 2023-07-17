using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Owner : Entity
{
    [Required]
	[MaxLength(255)]
	public string Name { get; init; }
	
	public List<Area> Areas { get; set; }
}