using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Key : Entity
{
    public required string Pass { get; init; }
	
	public required List<Role> Roles { get; set; }
}