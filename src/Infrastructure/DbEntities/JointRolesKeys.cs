namespace Infrastructure.DbEntities;

public class DbRolesKeys : DbJointEntity
{
	// JOINING TABLE Role <-> Key
	public Guid RoleFId { get; set; }
	public RoleEntity RoleEntity { get; set;}
	
	public Guid KeyFId { get; set; }
	public KeyEntity KeyEntity { get; set; }
}