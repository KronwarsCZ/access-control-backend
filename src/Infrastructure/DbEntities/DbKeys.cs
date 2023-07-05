using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbKeys
{
	[Key]
	[Required]
	public int Id { get; set; }
	[Required]
	public Guid Guid { get; set; }
	
	[Required]
	public string Name { get; set; }
	[Required]
	public string Pass { get; set; }
}