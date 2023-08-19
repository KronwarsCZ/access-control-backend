using Core.Entities;

namespace Application.UnitOfWorkStuff_to_sort;

public interface IBasicCommandRepository<T>
    where T : Entity
{
    void Add(T entity, CancellationToken cancellationToken = default);
    
    void Update(T entity, CancellationToken cancellationToken = default);
    
    void Delete(T entity, CancellationToken cancellationToken = default);

    void HardDelete(T entity, CancellationToken cancellationToken = default);
}