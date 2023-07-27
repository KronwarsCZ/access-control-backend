using Core.Entities;

namespace Infrastructure.DbEntities;

public class DeviceEntity : DbEntity
{
    public required string Name { get; set; }

    public required  DeviceType DeviceType { get; set; }
    
    // A Device has these Roles
    public List<DbDevicesRoles> DbDevicesRoles { get; set; }

}