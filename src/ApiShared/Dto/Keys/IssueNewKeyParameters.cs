using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Keys;

public class IssueNewKeyParameters
{
    [Required]
    public Guid OwnerId { get; init; }
    public string Name { get; init; }
    public IEnumerable<Guid> RoleIds { get; init; }
}
