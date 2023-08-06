using Core.Entities;

namespace Application.Services;

public interface IBasicJointService<A, B>
where A : Entity
where B : Entity
{
    Task<IEnumerable<B>> GetAtoBAsync(A oneEntity, CancellationToken cancellationToken);
    Task<IEnumerable<A>> GetBtoAAsync(B oneEntity, CancellationToken cancellationToken);
    
    Task<Guid> AddAtoBAsync(A oneEntity, IEnumerable<B> manyEntities, CancellationToken cancellationToken);
    Task<Guid> AddBtoAAsync(B oneEntity, IEnumerable<A> manyEntities, CancellationToken cancellationToken);
    
    Task DeleteAtoBAsync(A oneEntity, IEnumerable<B> manyEntities, CancellationToken cancellationToken);
    Task DeleteBtoAAsync(B oneEntity, IEnumerable<A> manyEntities, CancellationToken cancellationToken);    
}
