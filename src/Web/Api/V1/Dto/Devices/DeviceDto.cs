using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace AccessControl.Web.Api.V1.Dto;

public sealed class DeviceDto : Dto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DeviceType Type { get; set; }
}