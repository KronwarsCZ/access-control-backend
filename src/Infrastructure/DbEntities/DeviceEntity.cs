using System;
using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Infrastructure.DbEntities;

public class DeviceEntity : DbEntity
{
    public required  DeviceType DeviceType { get; set; }
    
    // A Device has these Roles
    public required List<DbDevicesRoles> DbDevicesRoles { get; set; }

}