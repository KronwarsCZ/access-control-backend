using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class KeyDeleteParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<Guid> Ids { get; set; }
}
