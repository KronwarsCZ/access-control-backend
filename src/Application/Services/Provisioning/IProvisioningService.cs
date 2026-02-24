using ApiShared.Dto.IoT;
using Core.Entities.DeviceProvisioning;

namespace Application.Services.Provisioning;

/// <summary>
/// Service for handling device provisioning operations.
/// </summary>
public interface IProvisioningService
{
    /// <summary>
    /// Provisions a new device.
    /// </summary>
    /// <param name="request">Provisioning request with CSR and device info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provisioning response with signed certificate</returns>
    Task<ProvisioningResponse> ProvisionDeviceAsync(ProvisioningRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Renews an existing device's certificate.
    /// </summary>
    /// <param name="request">Renewal request with new CSR</param>
    /// <param name="clientCertificateCn">CN from the mTLS client certificate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Renewal response with new signed certificate</returns>
    Task<RenewalResponse> RenewCertificateAsync(RenewalRequest request, string clientCertificateCn, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a device has already been provisioned.
    /// </summary>
    /// <param name="deviceId">Device identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if device is already provisioned</returns>
    Task<bool> IsDeviceProvisionedAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a provisioned device by its device ID.
    /// </summary>
    /// <param name="deviceId">Device identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provisioned device or null</returns>
    Task<ProvisionedDevice?> GetProvisionedDeviceAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Validates a device ID format.
    /// </summary>
    /// <param name="deviceId">Device ID to validate</param>
    /// <returns>True if valid</returns>
    bool ValidateDeviceIdFormat(string deviceId);
}
