using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class GetDeviceByIdResult
{
    [Required]
    public DeviceResult Device { get; set; } = null!;
}
