namespace Application.IRepositories;

public interface IBasicRepository<T> 
    where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> AddAsync(IEnumerable<T> entitiesToAdd, CancellationToken cancellationToken);
    void Delete(IEnumerable<T> entitiesToDelete);
    void Update(IEnumerable<T> entitiesToUpdate);
}
