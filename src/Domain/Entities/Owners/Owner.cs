using Core.Entities.Accounts;
using Core.Entities.Areas;
using Core.Entities.BaseEntities;

namespace Core.Entities.Owners;

public class Owner : Entity
{
    private Owner()
    {
    }
    public string Name { get; set; } = null!;
    public List<Area> Areas { get; set; } = new();
    public List<Account> Accounts { get; set; } = new();
    
    public static Owner Create(string name)
    {
        var owner = CreateEntity<Owner>();
        owner.Name = name;
        return owner;
    }
}