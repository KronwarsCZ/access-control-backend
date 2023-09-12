using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public sealed class DeviceAddParameters
{
    [Required]
    public Guid OwnerId { get; set; }
    
    [Required]
    public string Name { get; set; }
}