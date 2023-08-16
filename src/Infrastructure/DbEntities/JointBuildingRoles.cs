namespace Infrastructure.DbEntities;

public class JointBudildingRoles : DbJointEntity
{
	// JOINING TABLE Building <-> Role
	public Guid BuildingId { get; set; }
    public BuildingEntity BuildingEntity { get; set; } = null!;
	
	public Guid RoleId { get; set; }
	public RoleEntity RoleEntity { get; set; } = null!;
}