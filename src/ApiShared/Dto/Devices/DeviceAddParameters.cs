using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public sealed class DeviceAddParameters
{
    [Required]
    public Guid OwnerId { get; set; }
    
    [Required]
    public string Name { get; set; }
}