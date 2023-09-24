namespace Application.Services.Messaging;

public interface IServiceBusReceiver
{
    Task FetchUpdateFirmwareMessages(CancellationToken cancellationToken);
}
