using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public sealed class DeviceAddParameters
{
    [Required]
    public string Name { get; set; } = null!;
}
