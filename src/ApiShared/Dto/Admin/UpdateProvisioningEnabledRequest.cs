namespace ApiShared.Dto.Admin;

/// <summary>
/// Request to enable or disable provisioning.
/// </summary>
public class UpdateProvisioningEnabledRequest
{
    /// <summary>
    /// Whether provisioning should be enabled.
    /// </summary>
    public bool Enabled { get; init; }
}
