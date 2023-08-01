using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public class DevicesRolesDeleteParameters<A, B>
    where A : class
    where B : class
{
    [Required]
    public DevicesRolesDto<A, B> DevicesRoles { get; set; }
}
