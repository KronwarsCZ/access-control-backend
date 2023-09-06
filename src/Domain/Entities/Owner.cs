using Framework.Identification;

namespace Core.Entities;

public class Owner : Entity
{
    public string Name { get; set; }
    public List<Area> Areas { get; set; } = new();
	
	public List<Account> Accounts { get; set; } = new();

    public List<Device> Devices { get; set; } = new();

    public static Owner Create(string name)
    {
        return new Owner
        {
            Id = SequentialGuid.Create(),
            Name = name
        };
    }
}