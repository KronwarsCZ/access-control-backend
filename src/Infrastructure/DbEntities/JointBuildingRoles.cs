namespace Infrastructure.DbEntities;

public class DbBudildingRoles : DbJointEntity
{
	// JOINING TABLE Building <-> Role
	public Guid BuildingFId { get; set; }
    public BuildingEntity BuildingEntity { get; set; }
	
	public Guid RoleFId { get; set; }
	public RoleEntity RoleEntity { get; set; }
}