using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class GetAllRolesForDeviceParameters
{
    [Required]
    public Guid DeviceId { get; init; }
}
