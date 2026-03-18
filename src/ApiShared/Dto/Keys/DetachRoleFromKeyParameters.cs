using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class DetachRoleFromKeyParameters
{
    [Required]
    public Guid KeyId { get; set; }
    
    [Required]
    public Guid RoleId { get; set; }
}
