namespace Core.Entities;

public class Building : Entity
{
    public required string Name { get; set; }

    public required string Address { get; init; }
    
	public required Area Area { get; set; }
	
	public required List<Role> Roles { get; set; }
}