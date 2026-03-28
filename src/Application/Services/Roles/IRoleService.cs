using System.Linq.Expressions;
using Application.Services.BasicService;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;

namespace Application.Services.Roles;

public interface IRoleService : IBasicService
{
    Task<IEnumerable<Role>> GetAllAsync(Owner owner, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Role> newRoleEnumerable, CancellationToken cancellationToken);
    Task DeleteAsync(IEnumerable<Role> roles, CancellationToken ctsToken);
    public Task<IEnumerable<Role>> FindByConditionAsync(Expression<Func<Role,bool>> predicate, CancellationToken cancellationToken);
    Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Key>?> GetKeysForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Device>?> GetDevicesForRoleAsync(Guid roleId, Guid ownerId, CancellationToken cancellationToken);
    Task RenameAsync(Role role, string newName, CancellationToken cancellationToken);
}
