namespace Infrastructure.DbEntities;

public class DbRolesKeys
{
	// JOINING TABLE Role <-> Key
	public Guid RoleFId { get; set; }
	public DbRole DbRole { get; set;}
	
	public Guid KeyFId { get; set; }
	public DbKey DbKey { get; set; }
}