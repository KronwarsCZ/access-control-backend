using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class DeviceDeleteParameters
{
    [Required]
    public Guid DeviceId { get; init; }
}
