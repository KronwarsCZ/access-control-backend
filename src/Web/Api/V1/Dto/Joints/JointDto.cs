using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class JointDto<A, B>
where A : Dto
where B : Dto
{
    [Required]
    public Guid FirstThingGuid { get; set; }
    
    [Required]
    public IEnumerable<Guid> ManyThingsGuids { get; set; }
}
