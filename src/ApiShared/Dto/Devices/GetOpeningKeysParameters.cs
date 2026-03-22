using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public class GetOpeningKeysParameters
{
    [Required]
    public Guid DeviceId { get; init; }
}
