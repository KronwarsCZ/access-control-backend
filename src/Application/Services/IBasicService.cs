namespace Application.Services;

public interface IBasicService<T>
where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> AddAsync(IEnumerable<T> entitiesToAdd, CancellationToken cancellationToken);
    void Delete(IEnumerable<T> entitiesToDelete, CancellationToken cancellationToken);
    void Update(IEnumerable<T> entitiesToUpdate, CancellationToken cancellationToken);
}
