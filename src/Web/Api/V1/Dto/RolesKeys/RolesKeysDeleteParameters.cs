using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class RolesKeysDeleteParameters<A, B>
    where A : Dto
    where B : Dto
{
    [Required]
    public JointDto<A, B> RolesKeys { get; set; }
}
