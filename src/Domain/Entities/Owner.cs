namespace Core.Entities;

public class Owner : Entity
{
    public required List<Area> Areas { get; set; }
	
	public List<Account> Accounts { get; set; }
}