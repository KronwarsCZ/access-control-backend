using System.ComponentModel.DataAnnotations;

namespace AccessControl.Web.Api.V1.Dto.Devices;

public class DeviceResult
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
}
