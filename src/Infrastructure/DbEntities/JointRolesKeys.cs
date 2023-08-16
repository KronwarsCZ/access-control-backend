namespace Infrastructure.DbEntities;

public class JointRolesKeys : DbJointEntity
{
	// JOINING TABLE Role <-> Key
	public Guid RoleId { get; set; }
	public RoleEntity RoleEntity { get; set; } = null!;
	
	public Guid KeyId { get; set; }
	public KeyEntity KeyEntity { get; set; } = null!;
}