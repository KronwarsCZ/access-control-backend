using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class DetachRoleFromDeviceParameters
{
    [Required]
    public Guid DeviceId { get; init; }
    
    [Required]
    public Guid RoleId { get; init; }
}
