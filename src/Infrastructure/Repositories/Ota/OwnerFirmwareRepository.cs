using Application.IRepositories.Ota;
using Core.Entities.Ota;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Ota;

public class OwnerFirmwareRepository : BasicRepository<OwnerFirmware>, IOwnerFirmwareRepository
{
    public OwnerFirmwareRepository(AccessControlDbContext context)
        : base(context)
    {
    }

    public async Task<OwnerFirmware?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return await Context.OwnerFirmwares
            .Where(f => f.OwnerId == ownerId && !f.IsDeleted)
            .Include(f => f.Owner)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<OwnerFirmware>> GetAllWithOwnerAsync(CancellationToken cancellationToken)
    {
        return await Context.OwnerFirmwares
            .Where(f => !f.IsDeleted)
            .Include(f => f.Owner)
            .ToListAsync(cancellationToken);
    }
}
