using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.RolesKeys;

public class RolesKeysAddParameters<A,B>
where A : Dto
where B : Dto
{
    [Required]
    public RolesKeysDto<A, B> RolesKeys { get; set; }
}
