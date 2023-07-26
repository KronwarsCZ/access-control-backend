namespace Infrastructure.DbEntities;

public class AreaEntity : DbEntity
{

    public string Location { get; set; }
	
    public required string Name { get; set; }

	public OwnerEntity Owner { get; set; }
	
	public List<BuildingEntity> Buildings { get; set; }
}
