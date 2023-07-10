using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbDevice
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    public string Owner { get; set; }
    
    // A Device has these Roles
    public ICollection<DbDevicesRoles> DbDevicesRoles { get; set; }

}