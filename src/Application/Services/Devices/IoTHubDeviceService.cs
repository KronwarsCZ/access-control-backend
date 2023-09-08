using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Serilog;

namespace Application.Services.Devices;

public sealed class IoTHubDeviceService : IIoTHubDeviceService
{


    public async Task RegisterDevice(Guid deviceId)
    {
        //var iotHubConnectionString =  Environment.GetEnvironmentVariable("CUSTOMCONNSTR_IOTHUB");
        var iotHubConnectionString = "HostName=AccessControl-IotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=RpnqnoYHeMxN+z3nAqqIZRWXEqJqwUTXQEgZrO5xvdE=";
        var stringIdentifier = deviceId.ToString();
        
        RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
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
}