using ApiShared.Dto.IoT;

namespace Application.Services.AccessList;

/// <summary>
/// Service for generating access lists (allowed UIDs) for lock devices.
/// </summary>
public interface IAccessListService
{
    /// <summary>
    /// Gets the allowed UID entries for a specific device.
    /// </summary>
    /// <param name="deviceId">Device identifier (e.g., DEV-AABBCCDDEEFF)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of allowed UID entries</returns>
    Task<IEnumerable<AllowedUidEntry>> GetAllowedUidsAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Generates the binary format allowlist for a device.
    /// Binary format is optimized for ESP32 memory storage.
    /// </summary>
    /// <param name="deviceId">Device identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary data containing header and entries</returns>
    Task<byte[]> GenerateBinaryAllowlistAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the JSON format response for allowed UIDs.
    /// </summary>
    /// <param name="deviceId">Device identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JSON response object</returns>
    Task<AllowedUidsResponse> GetAllowedUidsResponseAsync(string deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Validates a device ID format.
    /// </summary>
    /// <param name="deviceId">Device ID to validate</param>
    /// <returns>True if valid format</returns>
    bool ValidateDeviceId(string deviceId);

    /// <summary>
    /// Finds a device by its device ID string (e.g., DEV-AABBCCDDEEFF).
    /// </summary>
    /// <param name="deviceId">Device identifier string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Device GUID if found, null otherwise</returns>
    Task<Guid?> FindDeviceGuidByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
}
