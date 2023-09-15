using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class AttachRoleToDeviceParameters
{
    [Required]
    public Guid DeviceId { get; init; }
    
    [Required]
    public Guid RoleId { get; init; }
}
