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

	public IList<Device> Devices { get; set; }
	public IList<Key> Keys { get; set; }
}