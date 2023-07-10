using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbArea
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
	
	public DbOwner Owner { get; set; }
	
	public ICollection<DbBuilding> Buildings { get; set; }
}