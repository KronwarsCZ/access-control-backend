using Application.UnitOfWorkStuff_to_sort;

namespace Application.Services.RolesKeys;

public class RolesKeysService : IRolesKeysService
{
    private readonly IUnitOfWork context;

    public RolesKeysService(IUnitOfWork context)
    {
        this.context = context;
    }
}
