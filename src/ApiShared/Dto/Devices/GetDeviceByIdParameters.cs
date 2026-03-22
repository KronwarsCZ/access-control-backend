using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class GetDeviceByIdParameters
{
    [Required]
    public Guid Id { get; set; }
}
