using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Role : Entity
{
    [Required]
	[MaxLength(255)]
	public string Name { get; init; }

	public List<Device> Devices { get; set; }

	public List<Key> Keys { get; set; }

	public List<Building> Buildings { get; set; }
}