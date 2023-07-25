namespace Application.Services;

public interface IBasicService<T>
where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> AddAsync(IEnumerable<T> things, CancellationToken cancellationToken);
    Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<T> UpdateAsync(Guid id, T thingToUpdate, CancellationToken cancellationToken);
}
