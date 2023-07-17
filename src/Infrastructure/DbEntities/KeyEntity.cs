using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class KeyEntity : DbEntity
{ 
    public string Pass { get; set; }
	
	// A key is has these Roles
	public List<JoinRolesKeys> DbRolesKeys { get; set; }
}