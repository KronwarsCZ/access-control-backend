namespace ApiShared.Dto.Roles;

public class GetAllRolesResult
{
    public IEnumerable<RoleResult> Roles { get; init; } = null!;
}
