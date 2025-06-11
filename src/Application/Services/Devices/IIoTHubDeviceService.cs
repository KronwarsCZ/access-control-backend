namespace Application.Services.Devices;
using Azure.Messaging.EventGrid;


public interface IIoTHubDeviceService
{
    Task RegisterDeviceToIotHub(Guid deviceId);
    
    Task NotifiedOfNewDevice(EventGridEvent[] events, CancellationToken cancellationToken);
}
