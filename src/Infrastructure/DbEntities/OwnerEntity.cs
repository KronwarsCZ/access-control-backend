using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class OwnerEntity : DbEntity
{
    public List<AreaEntity> Areas { get; set; }
    
    public List<AccountEntity> Accounts { get; set; }
}