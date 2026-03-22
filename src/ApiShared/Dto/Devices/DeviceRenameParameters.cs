using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Devices;

public sealed class DeviceRenameParameters
{
    [Required]
    public Guid DeviceId { get; set; }

    [Required]
    public string NewName { get; set; } = null!;
}
