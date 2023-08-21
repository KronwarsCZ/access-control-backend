
namespace Infrastructure.DbEntities;

public class DeviceEntity : DbEntity
{
    public required string Name { get; set; }

    public required OwnerEntity Owner { get; init; }
    public required Guid OwnerId { get; init; }

    public required DeviceTypeEntity Type { get; init; }
    
    // A Device has these Roles
    public List<DbDevicesRoles> DevicesRolesEntity { get; set; }
}