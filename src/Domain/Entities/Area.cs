using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Area
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Location { get; set; }
	
	
	public Owner Owner { get; set; }
	public ICollection<Building> Buildings { get; set; }
}