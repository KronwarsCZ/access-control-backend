namespace ApiShared.Dto.Admin;

/// <summary>
/// Summary of a per-owner provisioning secret (the secret itself is never returned).
/// </summary>
public class ProvisioningSecretResponse
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public string? Label { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
}
