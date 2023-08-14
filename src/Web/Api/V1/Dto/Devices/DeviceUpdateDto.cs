using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto;

public class DeviceUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    public DeviceType Type { get; set; }
    
    public string Name { get; set; }
}
