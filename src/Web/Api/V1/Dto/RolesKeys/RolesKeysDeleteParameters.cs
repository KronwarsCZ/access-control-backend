using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.RolesKeys;

public class RolesKeysDeleteParameters<A, B>
    where A : Dto
    where B : Dto
{
    [Required]
    public RolesKeysDto<A, B> RolesKeys { get; set; }
}
