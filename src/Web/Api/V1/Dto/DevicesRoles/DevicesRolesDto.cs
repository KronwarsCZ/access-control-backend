using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class DevicesRolesDto<A,B>
where A : class
where B : class
{
    [Required]
    public A oneThing { get; set; }
    
    [Required]
    public IEnumerable<B> manyThings { get; set; }
}
