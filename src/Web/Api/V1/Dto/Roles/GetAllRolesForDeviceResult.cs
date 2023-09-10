namespace AccessControl.Web.Api.V1.Dto;

public class GetAllRolesForDeviceResult
{
    public IEnumerable<RoleResult> Roles { get; init; }
}

public class RoleResult
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}