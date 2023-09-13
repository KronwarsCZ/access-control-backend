using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class IssueNewKeyParameters
{
    [Required]
    public Guid OwnerId { get; init; }
    public string Name { get; init; }
    public IEnumerable<Guid> RoleIds { get; init; }
}
