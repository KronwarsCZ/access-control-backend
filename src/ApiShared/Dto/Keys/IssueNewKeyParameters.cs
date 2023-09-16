using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class IssueNewKeyParameters
{
    [Required]
    public string Name { get; init; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<Guid> RoleIds { get; init; }
    
    public DateTime ValidityStartUtc { get; init; }
    
    public DateTime? ValidityEndUtc { get; init; }
}
