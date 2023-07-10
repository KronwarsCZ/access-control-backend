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

	public ICollection<Device> Devices { get; set; }
	public ICollection<Key> Keys { get; set; }
	public ICollection<Building> Buildings { get; set; }
}