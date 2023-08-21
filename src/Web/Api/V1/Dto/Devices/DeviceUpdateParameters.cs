using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public class DeviceUpdateParameters
{
    // rekneme, ze chceme davat najednou max 100 klicu
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<DeviceUpdateDto> Devices { get; set; }
}
