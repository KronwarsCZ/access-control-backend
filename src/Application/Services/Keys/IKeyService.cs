using Application.Services.BasicService;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Application.Services.Keys;

public interface IKeyService : IBasicService
{
    Task<IEnumerable<Key>> GetAllAsync(Owner owner, CancellationToken cancellationToken);
    Task<Key> GetByIdAsync(Guid keyId, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Key> newKeyEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(Key key, CancellationToken cancellationToken);
    Task IssueNewKey(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken); 
    Task<IEnumerable<Device>> GetDevicesToOpen(Key key, CancellationToken cancellationToken);
    Task AttachRoleToKey(Key key, Role role, CancellationToken cancellationToken);
    Task DetachRoleFromKey(Key key, Role role, CancellationToken cancellationToken);
    Task RenameAsync(Key key, string newName, CancellationToken cancellationToken);
}