using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public class GetAllDevicesParameters
{
    [Required]
    public Guid OwnerId { get; set; }
}
