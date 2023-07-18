using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class BuildingEntity : DbEntity
{
    public string Address { get; set; }
	
	
	public AreaEntity AreaEntity { get; set; }
	
	public List<DbBudildingRoles> DbBudildingRoles { get; set; }
}