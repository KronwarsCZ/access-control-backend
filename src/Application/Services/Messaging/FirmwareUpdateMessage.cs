namespace Application.Services.Messaging;

public class FirmwareUpdateMessage
{
    public Guid DeviceId { get; init; }

    public Guid FirmwareId { get; init; }
}
