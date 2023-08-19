using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Roles;

public class RoleDeleteParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<Guid> Ids { get; set; }
}
