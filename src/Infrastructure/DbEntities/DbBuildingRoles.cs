namespace Infrastructure.DbEntities;

public class DbBudildingRoles
{
	// JOINING TABLE Building <-> Role
	public Guid BuildingFId { get; set; }
	public DbBuilding DbBuilding { get; set; }
	
	public Guid RoleFId { get; set; }
	public DbRole DbRole { get; set; }
}