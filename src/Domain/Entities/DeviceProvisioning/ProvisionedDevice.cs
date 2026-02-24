#nullable enable
using Core.Entities.BaseEntities;

namespace Core.Entities.DeviceProvisioning;

/// <summary>
/// Tracks a provisioned IoT device with its certificate information.
/// </summary>
public class ProvisionedDevice : Entity
{
    private ProvisionedDevice() { }

    /// <summary>
    /// Device identifier string (e.g., DEV-AABBCCDDEEFF).
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Serial number of the current certificate.
    /// </summary>
    public required string CertificateSerialNumber { get; set; }

    /// <summary>
    /// Thumbprint/fingerprint of the current certificate.
    /// </summary>
    public required string CertificateThumbprint { get; set; }

    /// <summary>
    /// When the current certificate was issued.
    /// </summary>
    public DateTime CertificateIssuedUtc { get; set; }

    /// <summary>
    /// When the current certificate expires.
    /// </summary>
    public DateTime CertificateExpiresUtc { get; set; }

    /// <summary>
    /// Number of times this device has renewed its certificate.
    /// </summary>
    public int RenewalCount { get; set; }

    /// <summary>
    /// Last time the device contacted the server.
    /// </summary>
    public DateTime? LastSeenUtc { get; set; }

    /// <summary>
    /// Whether the device is currently active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Reference to the linked Device entity if exists.
    /// </summary>
    public Guid? LinkedDeviceId { get; set; }
    
    public Devices.Device? LinkedDevice { get; set; }

    public static ProvisionedDevice Create(
        string deviceId, 
        string certificateSerialNumber,
        string certificateThumbprint,
        DateTime issuedUtc,
        DateTime expiresUtc)
    {
        var device = CreateEntity<ProvisionedDevice>();
        device.DeviceId = deviceId;
        device.CertificateSerialNumber = certificateSerialNumber;
        device.CertificateThumbprint = certificateThumbprint;
        device.CertificateIssuedUtc = issuedUtc;
        device.CertificateExpiresUtc = expiresUtc;
        device.RenewalCount = 0;
        device.IsActive = true;
        return device;
    }
}
