using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Role
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }

	public List<Device> Devices { get; set; }
	public List<Key> Keys { get; set; }
	public List<Building> Buildings { get; set; }
}