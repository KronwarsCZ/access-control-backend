using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to update OTA firmware version.
/// </summary>
public class UpdateOtaSettingsRequest
{
    [Required]
    [RegularExpression(@"^\d+\.\d+\.\d+$", ErrorMessage = "Version must be in format X.Y.Z")]
    [MaxLength(31)]
    public required string FirmwareVersion { get; init; }
}
