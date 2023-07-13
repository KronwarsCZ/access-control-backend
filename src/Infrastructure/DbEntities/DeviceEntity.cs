using System;
using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Infrastructure.DbEntities;

public class DeviceEntity
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType DeviceType { get; set; }
    
    // A Device has these Roles
    public List<DbDevicesRoles> DbDevicesRoles { get; set; }

}