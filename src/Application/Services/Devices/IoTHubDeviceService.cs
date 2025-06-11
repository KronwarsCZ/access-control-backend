using Azure.Messaging.EventGrid;
using Framework.Utilities;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Serilog;

namespace Application.Services.Devices;

public sealed class IoTHubDeviceService : IIoTHubDeviceService
{
    private RegistryManager registryManager;
    
    public IoTHubDeviceService()
    {
        var iotHubConnectionString =  Environment.GetEnvironmentVariable("CUSTOMCONNSTR_IOTHUB");
        Check.Contract(iotHubConnectionString, "Internal IotHub connection string is empty.");
        registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

    }

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
            var device = await registryManager.AddDeviceAsync(new Device(stringIdentifier));
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
            await registryManager.CloseAsync();
            Log.Debug("Finished registration of device with ID: {DeviceId}", stringIdentifier);
        }
    }

    public Task NotifiedOfNewDevice(EventGridEvent[] events, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}