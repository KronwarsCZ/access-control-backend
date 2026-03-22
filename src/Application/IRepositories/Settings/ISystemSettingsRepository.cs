using Core.Entities.Settings;

namespace Application.IRepositories.Settings;

/// <summary>
/// Repository interface for system settings operations.
/// </summary>
public interface ISystemSettingsRepository : IBasicRepository<SystemSettings>
{
    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">Setting key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Setting or null if not found</returns>
    Task<SystemSettings?> GetByKeyAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Sets a setting value, creating if it doesn't exist.
    /// </summary>
    /// <param name="key">Setting key</param>
    /// <param name="value">Setting value</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync(string key, string? value, string? description, CancellationToken cancellationToken);
}
