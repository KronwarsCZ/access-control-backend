namespace AccessControl.Web.Api.V1.Dto.Keys;

public class IssueNewKeyParameters
{
    public string Name { get; init; }
    public IEnumerable<Guid> RoleIds { get; init; }
}
