using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class BuildingEntity
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
	
	
	public AreaEntity AreaEntity { get; set; }
	
	public List<DbBudildingRoles> DbBudildingRoles { get; set; }
}