#nullable enable
using Core.Entities.BaseEntities;
using Core.Entities.Owners;

namespace Core.Entities.DeviceProvisioning;

/// <summary>
/// Per-owner provisioning secret. When a device provisions with a shared secret,
/// the system hashes it, looks it up here, and resolves the owning tenant.
/// </summary>
public class ProvisioningSecret : Entity
{
    private ProvisioningSecret() { }

    /// <summary>
    /// SHA-256 hash (base-64) of the plaintext secret.
    /// </summary>
    public required string SecretHash { get; set; }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    /// <summary>
    /// Optional friendly name (e.g. "Factory Floor A").
    /// </summary>
    public string? Label { get; set; }

    public bool IsActive { get; set; }

    public static ProvisioningSecret Create(string secretHash, Guid ownerId, string? label = null)
    {
        var entity = CreateEntity<ProvisioningSecret>();
        entity.SecretHash = secretHash;
        entity.OwnerId = ownerId;
        entity.Label = label;
        entity.IsActive = true;
        return entity;
    }
}
