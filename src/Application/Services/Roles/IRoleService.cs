using Core.Entities;

namespace Application.Services;

public interface IRoleService : IBasicService<Role>
{
    Role Create(string name);
}
