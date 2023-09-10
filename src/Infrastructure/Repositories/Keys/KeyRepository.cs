using Application.IRepositories.Keys;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Keys;

public class KeyRepository : BasicRepository<Key>, IKeyRepository
{
    public KeyRepository(AccessControlDbContext context)
        : base(context)
    {
    }
}
