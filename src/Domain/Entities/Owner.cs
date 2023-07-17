using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Owner : Entity
{
    public required string Name { get; init; }
	
	public required List<Area> Areas { get; set; }
}