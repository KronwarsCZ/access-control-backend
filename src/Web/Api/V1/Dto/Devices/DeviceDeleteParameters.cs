using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public class DeviceDeleteParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<Guid> Ids { get; set; }
}
