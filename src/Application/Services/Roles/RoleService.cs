using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;

namespace Application.Services.Roles;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork context)
    {
        this._unitOfWork = context;
    }

    public async Task Delete(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        _unitOfWork.RoleRepository.Delete(roles);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.RoleRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken)
    {
        _unitOfWork.RoleRepository.Add(rolesToAdd);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
