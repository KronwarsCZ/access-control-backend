using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Device
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    public string Owner { get; set; }
}
