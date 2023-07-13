using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class AreaEntity
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
	
	public OwnerEntity Owner { get; set; }
	
	public List<BuildingEntity> Buildings { get; set; }
}