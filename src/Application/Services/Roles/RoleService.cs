using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Identification;

namespace Application.Services.Roles;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork context;

    public RoleService(IUnitOfWork context)
    {
        this.context = context;
    }


    public async Task Delete(IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        context.RoleQueryRepository.Delete(roles);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.RoleQueryRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(IEnumerable<Role> rolesToAdd, CancellationToken cancellationToken)
    {
        context.RoleCommandRepository.Add(rolesToAdd, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
