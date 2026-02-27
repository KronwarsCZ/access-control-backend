namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to enable or disable OTA updates.
/// </summary>
public class UpdateOtaEnabledRequest
{
    /// <summary>
    /// Whether OTA should be enabled.
    /// </summary>
    public bool Enabled { get; init; }
}
