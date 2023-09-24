namespace Application.Services.Messaging;

public interface IServiceBusPublisher
{
    Task SendUpdateFirmwareMessage(FirmwareUpdateMessage payload);
}
