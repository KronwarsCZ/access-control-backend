using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DbEntities;

public class DbDevice
{
    [Required]
    public Guid Guid { get; set; }
    public string Owner { get; set; }
    
    [Key]
    [Required]
    public int Id { get; set; }
    public string RolesFId { get; set; }
}