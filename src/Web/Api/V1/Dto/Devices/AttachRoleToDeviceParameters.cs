using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public class AttachRoleToDeviceParameters
{
    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public Guid DeviceId { get; init; }
    
    [Required]
    public Guid RoleId { get; init; }
}
