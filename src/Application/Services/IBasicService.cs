namespace Application.Services;

public interface IBasicService<T>
where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> AddAsync(IEnumerable<T> entitiesToAdd, CancellationToken cancellationToken);
    Task Delete(IEnumerable<T> entitiesToDelete, CancellationToken cancellationToken);
    Task Update(IEnumerable<T> entitiesToUpdate, CancellationToken cancellationToken);
}
