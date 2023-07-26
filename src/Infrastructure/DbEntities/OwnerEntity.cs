namespace Infrastructure.DbEntities;

public class OwnerEntity : DbEntity
{
    public required string Name { get; set; }

    
    public List<AreaEntity> Areas { get; set; }
    
    public List<AccountEntity> Accounts { get; set; }
}