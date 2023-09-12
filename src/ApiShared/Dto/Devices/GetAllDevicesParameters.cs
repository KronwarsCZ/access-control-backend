using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class GetAllDevicesParameters
{
    [Required]
    public Guid OwnerId { get; set; }
}
