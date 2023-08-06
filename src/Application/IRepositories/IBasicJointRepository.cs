namespace Application.IRepositories;

public interface IBasicJointRepository<A, B>
    where A : class
    where B : class
{
    // we want to have functions for both ways
    // one A entity wants multiple B entities || one B entity wants multiple A entities 

    Task<IEnumerable<B>> GetAtoBAsync(A oneEntity, CancellationToken cancellationToken);
    Task<IEnumerable<A>> GetBtoAAsync(B oneEntity, CancellationToken cancellationToken);

    // uhhh co tady vracet? guid?
    Task<Guid> AddAtoBAsync(A oneEntity, IEnumerable<B> manyEntities, CancellationToken cancellationToken);
    Task<Guid> AddBtoAAsync(B oneEntity, IEnumerable<A> manyEntities, CancellationToken cancellationToken);
    
    void DeleteAtoB(A oneEntity, IEnumerable<B> manyEntities);
    void DeleteBtoA(B oneEntity, IEnumerable<A> manyEntities);
}
