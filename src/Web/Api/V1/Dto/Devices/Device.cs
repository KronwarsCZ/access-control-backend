using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class DeviceDto
{
    [Required]
    public string Owner { get; set; }
}