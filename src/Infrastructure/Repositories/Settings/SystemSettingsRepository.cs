using Application.IRepositories.Settings;
using Core.Entities.Settings;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Settings;

/// <summary>
/// Repository implementation for system settings operations.
/// </summary>
public class SystemSettingsRepository : BasicRepository<SystemSettings>, ISystemSettingsRepository
{
    public SystemSettingsRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<SystemSettings?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return await Context.SystemSettings
            .Where(s => s.Key == key && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, string? value, string? description, CancellationToken cancellationToken)
    {
        var existing = await GetByKeyAsync(key, cancellationToken);

        if (existing != null)
        {
            existing.Value = value;
            existing.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            var setting = SystemSettings.Create(key, value, description);
            Context.SystemSettings.Add(setting);
        }
    }
}
