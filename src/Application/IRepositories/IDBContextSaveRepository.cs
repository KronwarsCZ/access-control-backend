namespace Application.IRepositories;

public interface IDBContextSaveRepository
{
    public Task<int> SaveContextAsync(CancellationToken cancellationToken);
}