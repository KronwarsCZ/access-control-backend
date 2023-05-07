using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class DeviceAddParameters
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IEnumerable<DeviceDto> Devices { get; set; }
}