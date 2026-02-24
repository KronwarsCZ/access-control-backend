using Core.Entities.DeviceProvisioning;

namespace Application.IRepositories.DeviceProvisioning;

/// <summary>
/// Repository interface for provisioned device operations.
/// </summary>
public interface IProvisionedDeviceRepository : IBasicRepository<ProvisionedDevice>
{
    /// <summary>
    /// Gets a provisioned device by its device ID string.
    /// </summary>
    /// <param name="deviceId">Device identifier (e.g., DEV-AABBCCDDEEFF)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provisioned device or null</returns>
    Task<ProvisionedDevice?> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a provisioned device by certificate thumbprint.
    /// </summary>
    /// <param name="thumbprint">Certificate thumbprint</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provisioned device or null</returns>
    Task<ProvisionedDevice?> GetByCertificateThumbprintAsync(string thumbprint, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all active provisioned devices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active provisioned devices</returns>
    Task<IEnumerable<ProvisionedDevice>> GetAllActiveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets devices with certificates expiring within the specified days.
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of devices with expiring certificates</returns>
    Task<IEnumerable<ProvisionedDevice>> GetExpiringCertificatesAsync(int days, CancellationToken cancellationToken);
}
