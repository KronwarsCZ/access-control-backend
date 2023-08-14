using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class DevicesRolesAddParameters<A, B>
where A : Dto
where B : Dto
{
    [Required]
    public JointDto<A, B> DevicesRoles { get; set; }
}
