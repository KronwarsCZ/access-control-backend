namespace Core.Entities;

public class Owner : Entity
{
    public string Name { get; set; }
    public List<Area> Areas { get; set; }
	
	public List<Account> Accounts { get; set; }
    
    public List<Device> Devices { get; set; }

    public static Owner Create(string name)
    {
        return new Owner
        {
            Name = name
        };
    }
}