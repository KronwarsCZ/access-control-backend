namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to enable or disable OTA updates.
/// </summary>
public class UpdateOtaEnabledRequest
{
    public bool Enabled { get; init; }
}
