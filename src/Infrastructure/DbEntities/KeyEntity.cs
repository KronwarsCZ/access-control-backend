namespace Infrastructure.DbEntities;

public class KeyEntity : DbEntity
{ 
    public string Pass { get; set; }
    public string Name { get; set; }
    
    // A key is has these Roles
    public List<RoleEntity> Roles { get; } = new();
    public List<JointRolesKeys> DbRolesKeys { get; } = new();

}