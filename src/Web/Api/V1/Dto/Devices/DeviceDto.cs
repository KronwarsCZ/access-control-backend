using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public sealed class DeviceDto : Dto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public bool IsDeleted { get; set; }
    
    [Required]
    public DeviceType Type { get; set; }
}