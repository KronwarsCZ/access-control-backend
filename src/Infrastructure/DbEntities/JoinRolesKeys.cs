namespace Infrastructure.DbEntities;

public class JoinRolesKeys
{
	// JOINING TABLE Role <-> Key
	public Guid RoleFId { get; set; }
	public RoleEntity DbRole { get; set;}
	
	public Guid KeyFId { get; set; }
	public KeyEntity KeyEntity { get; set; }
}