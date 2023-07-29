using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto;

public class DeviceUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DeviceType Type { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public Owner Owner { get; set; }
}
