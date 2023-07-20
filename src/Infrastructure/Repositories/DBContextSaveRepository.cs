using Application.IRepositories;
using Infrastructure.Database;

namespace Infrastructure.Repositories;

public class DBContextSaveRepository : IDBContextSaveRepository
{
    private readonly AccessControlDbContext context;
    public DBContextSaveRepository(AccessControlDbContext context)
    {
        this.context = context;
    }
    
    public Task<int> SaveContextAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}