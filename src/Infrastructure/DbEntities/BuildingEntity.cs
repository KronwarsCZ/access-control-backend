namespace Infrastructure.DbEntities;

public class BuildingEntity : DbEntity
{
    public string Address { get; set; }
	
    public required string Name { get; set; }

	
	public AreaEntity AreaEntity { get; set; }
	
    // A Building has there Roles
    public List<RoleEntity> Roles { get; } = new();
	public List<JointBudildingRoles> DbBuildingRoles { get; } = new();
}