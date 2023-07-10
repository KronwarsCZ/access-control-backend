using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbOwner
{
	// https://dbdiagram.io/d/64ac711202bd1c4a5ed4003c
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	public ICollection<DbArea> Areas { get; set; }
}