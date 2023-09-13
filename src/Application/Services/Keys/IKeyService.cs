using Application.Services.BasicService;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Application.Services.Keys;

public interface IKeyService : IBasicService
{
    Task<IEnumerable<Key>> GetAllAsync(CancellationToken cancellationToken);
    Task<Key> GetByIdAsync(Guid keyId, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(Key key, CancellationToken cancellationToken);
    Task IssueNewKey(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken);
}