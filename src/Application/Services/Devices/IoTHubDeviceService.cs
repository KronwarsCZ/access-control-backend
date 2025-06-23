#nullable enable
using Azure.Messaging.EventGrid;
using Framework.Utilities;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Serilog;
using Core.Entities;
using Core.Entities.Devices;
using Core.Entities.Owners;
using Application.IRepositories.Owners;
using Application.IRepositories.Devices;
using Application.Services.BasicService;
using Azure.Messaging.EventGrid.SystemEvents;
using Core.Entities.DeviceIoTInfo;
using Framework.Extensions;
using Device = Core.Entities.Devices.Device;

namespace Application.Services.Devices;

public sealed class IoTHubDeviceService : IIoTHubDeviceService
{
    private readonly RegistryManager registryManager;
    private readonly IDeviceRepository deviceRepository;
    private readonly IOwnerRepository ownerRepository;
    private readonly IUnitOfWork.IUnitOfWork unitOfWork;
    private readonly IIoTHubDeviceRepository ioTHubDeviceRepository;
    
    public IoTHubDeviceService(
        IDeviceRepository deviceRepository,
        IOwnerRepository ownerRepository,
        IUnitOfWork.IUnitOfWork unitOfWork,
        IIoTHubDeviceRepository ioTHubDeviceRepository)
    {
        var iotHubConnectionString = Environment.GetEnvironmentVariable("CONNSTR_IOTHUB");
        Check.Contract(iotHubConnectionString, "Internal IotHub connection string is empty.");
        registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        this.deviceRepository = deviceRepository;
        this.ownerRepository = ownerRepository;
        this.unitOfWork = unitOfWork;
        this.ioTHubDeviceRepository = ioTHubDeviceRepository;
    }

    // Deprecated
    public async Task RegisterDeviceToIotHub(Guid deviceId)
    {
        // temp reseni, aby se nevolal ostry iothub
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            return;
        }
        
        var stringIdentifier = deviceId.ToString();
        
        try
        {
            var device = await registryManager.AddDeviceAsync(new Microsoft.Azure.Devices.Device(stringIdentifier));
            Log.Debug("Device registered successfully with ID: {DeviceId}", device.Id);
        }
        catch (DeviceAlreadyExistsException)
        {
            Log.Error("Device already exists. Skipping registration");
        }
        catch (Exception ex)
        {
            Log.Error("Error registering the device: {ExMessage}", ex.Message);
        }
        finally
        {
            //await registryManager.CloseAsync();
            Log.Debug("Finished registration of device with ID: {DeviceId}", stringIdentifier);
        }
    }
    
    public async Task<SubscriptionValidationResponse?> EventRouter(EventGridEvent e, CancellationToken cancellationToken)
    {
        // Handle Azure Webhook validation
        if (e.TryGetSystemEventData(out var eventData))
        {
            switch (eventData)
            {
                case SubscriptionValidationEventData subscriptionValidationEventData:
                    Log.Information($"Got SubscriptionValidation event data, validation code: {subscriptionValidationEventData.ValidationCode}, topic: {e.Topic}");
                    // Only return response for subscription validation
                    return HandleSubscriptionValidation(subscriptionValidationEventData);

                case IotHubDeviceCreatedEventData deviceCreatedEventData:
                    await NotifiedOfNewDevice(deviceCreatedEventData, cancellationToken);
                    return null;

                default:
                    Log.Information($"Received event data of type: {eventData.GetType().Name}");
                    return null;
            }
        }
        
        // Handle custom events here
        Log.Information($"Received custom event of type: {e.EventType}");
        return null;
    }
    
    public SubscriptionValidationResponse HandleSubscriptionValidation(SubscriptionValidationEventData eventData)
    {
        SubscriptionValidationResponse response = new()
        {
            ValidationResponse = eventData.ValidationCode
        };
        return response;
    }
    
    public async Task NotifiedOfNewDevice(IotHubDeviceCreatedEventData e, CancellationToken cancellationToken)
    {
        try
        {
            // Get device ID from the event data
            if (string.IsNullOrEmpty(e.DeviceId))
            {
                Log.Error("Device ID is missing from event data");
                return;
            }

            // Get device twin to get the OwnerId
            var twin = await registryManager.GetTwinAsync(e.DeviceId, cancellationToken);
            var ownerId = twin.Properties.Desired["OwnerId"]?.ToString();
            var ownerGuid = Guid.Empty;
            
            if (string.IsNullOrEmpty(ownerId) || !Guid.TryParse(ownerId, out ownerGuid))
            {
                Log.Error("Invalid or missing OwnerId in device twin for device: {DeviceId}", e.DeviceId);
                return;
            }

            // Get owner from repository
            var owner = await ownerRepository.GetByIdAsync(ownerGuid, cancellationToken);
            if (owner == null)
            {
                Log.Error("Owner not found for ID: {OwnerId}", ownerGuid);
                return;
            }

            // Create new device
            var device = Device.Create(owner, $"Lock Device {e.DeviceId}", DeviceType.Lock);
            deviceRepository.Add(device.ToEnumerable());

            // Create DeviceIoTInfo
            DeviceIoTInfo deviceIoTInfo = DeviceIoTInfo.Create(
                owner,
                device,
                e.DeviceId,
                ConnectionState.Disconnected,
                twin.Properties.Reported["firmwareVersion"]?.ToString()
            );
            ioTHubDeviceRepository.Add(deviceIoTInfo.ToEnumerable());

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            Log.Information("Successfully created device and IoT info for device: {DeviceId}", e.DeviceId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing device creation event");
            throw;
        }
    }
}