using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public Role Create(Guid id, string name, bool isDeleted)
    {
        return new Role()
        {
            Id = id,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            Name = name,
            IsDeleted = isDeleted
        };
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _roleRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> AddAsync(IEnumerable<Role> things, CancellationToken cancellationToken)
    {
        return await _roleRepository.AddAsync(things, cancellationToken);
    }

    public async Task<(int, IEnumerable<string>)> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _roleRepository.DeleteAsync(ids, cancellationToken);
    }

    public async Task<Role> UpdateAsync(Guid id, Role role, CancellationToken cancellationToken)
    {
        return await _roleRepository.UpdateAsync(id, role, cancellationToken);
    }
}
