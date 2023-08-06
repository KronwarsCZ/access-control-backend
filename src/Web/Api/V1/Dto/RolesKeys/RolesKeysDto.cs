using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class RolesKeysDto<A,B>
    where A : Dto
    where B : Dto
{
    [Required]
    public A OneThing { get; set; }
    
    [Required]
    public IEnumerable<B> ManyThings { get; set; }
}
