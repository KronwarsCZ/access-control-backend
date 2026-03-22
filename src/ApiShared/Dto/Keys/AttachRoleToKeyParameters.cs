using System.ComponentModel.DataAnnotations;

namespace ApiShared.Dto.Keys;

public class AttachRoleToKeyParameters
{
    [Required]
    public Guid KeyId { get; set; }
    
    [Required]
    public Guid RoleId { get; set; }
}
