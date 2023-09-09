using Core.Entities.Areas;
using Core.Entities.BaseEntities;
using Core.Entities.Roles;

namespace Core.Entities.Buildings;

public class Building : Entity
{
    public required string Name { get; set; }

    public required string Address { get; init; }
    
	public required Area Area { get; set; }
	
	public required List<Role> Roles { get; set; }
}