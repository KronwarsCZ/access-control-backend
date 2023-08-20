using Core.Entities;

namespace Application.IRepositories.Keys;

public interface IKeyCommandRepository
{
    IEnumerable<Guid> Add(IEnumerable<Key> keys);
    void Update(IEnumerable<Key> keys);
    void Delete(IEnumerable<Key> keys);
}
