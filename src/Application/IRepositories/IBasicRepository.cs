namespace Application.IRepositories;

public interface IBasicRepository<T> 
    where T : class
{
    Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken);
    Task<T> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> AddAsync(IEnumerable<T> entitiesToAdd, CancellationToken cancellationToken);
    void Delete(IEnumerable<T> entitiesToDelete);
    void Update(IEnumerable<T> entitiesToUpdate);
}
