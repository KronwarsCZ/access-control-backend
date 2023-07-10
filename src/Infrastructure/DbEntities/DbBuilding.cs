using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbBuilding
{
	[Key]
	[Required]
	public Guid Id { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Name { get; set; }
	
	[Required]
	[MaxLength(255)]
	public string Address { get; set; }
	
	
	public DbArea Area { get; set; }
	
	public ICollection<DbBudildingRoles> DbBudildingRoles { get; set; }
}