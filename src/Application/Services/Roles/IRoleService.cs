using Core.Entities;

namespace Application.Services.Roles;

public interface IRoleService : IBasicService<Role>
{
    Role Create(string name);
}
