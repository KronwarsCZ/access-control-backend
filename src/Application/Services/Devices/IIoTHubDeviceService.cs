#nullable enable
using Azure.Messaging.EventGrid.SystemEvents;

namespace Application.Services.Devices;
using Azure.Messaging.EventGrid;


public interface IIoTHubDeviceService
{
    Task RegisterDeviceToIotHub(Guid deviceId);

    Task<SubscriptionValidationResponse?> EventRouter(EventGridEvent e, CancellationToken cancellationToken);
    
    Task NotifiedOfNewDevice(IotHubDeviceCreatedEventData e, CancellationToken cancellationToken);

    SubscriptionValidationResponse HandleSubscriptionValidation(SubscriptionValidationEventData eventData);

}
