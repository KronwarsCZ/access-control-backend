namespace AccessControl.Web.Api.V1.Dto;

public class DeviceAddResult
{
    public Guid DeviceId { get; init; }
    
    public DeviceTypeDto Type { get; init; }
}
