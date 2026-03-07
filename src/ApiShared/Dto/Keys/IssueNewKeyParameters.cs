using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class IssueNewKeyParameters
{
    [Required]
    public string Name { get; init; }
    
    /// <summary>
    /// The RFID UID read from the physical keycard, as a hex string (e.g. "AABBCCDD").
    /// </summary>
    [Required]
    [RegularExpression(@"^[0-9A-Fa-f\s\-:]{2,32}$", ErrorMessage = "KeyUid must be a valid hex string (2-16 hex bytes).")]
    public string KeyUid { get; init; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<Guid> RoleIds { get; init; }
    
    public DateTime? ValidityStartUtc { get; init; }
    
    public DateTime? ValidityEndUtc { get; init; }
}
