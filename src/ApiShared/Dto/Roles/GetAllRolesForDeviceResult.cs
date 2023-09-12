namespace ApiShared.Dto.Roles;

public class GetAllRolesForDeviceResult
{
    public IEnumerable<RoleResult> Roles { get; init; }
}

public class RoleResult
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}