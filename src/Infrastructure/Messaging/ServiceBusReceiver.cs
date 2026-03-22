using Application.Services.Devices;
using Application.Services.Messaging;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.Messaging;

public class ServiceBusReceiver : IServiceBusReceiver
{
    private readonly IDeviceService _deviceService;

    public ServiceBusReceiver(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    private static string ConnectionString => Environment.GetEnvironmentVariable("SQLCONNSTR_azor")!;
    private const string QueueName = "firmware-update-queue";


    public async Task FetchUpdateFirmwareMessages(CancellationToken cancellationToken)
    {
        await using var client = new ServiceBusClient(ConnectionString!);
        var receiver = client.CreateReceiver(QueueName);

        try
        {
            var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
            var commandData = JsonSerializer.Deserialize<FirmwareUpdateMessage>(message.Body);
            if (commandData is null)
            {
                Log.Warning("Failed to deserialize firmware update message from service bus.");
                return;
            }

            await _deviceService.DeviceUpdateFirmwareAsync(commandData, cancellationToken);
            Log.Warning($"Device {commandData.DeviceId} was updated to firmware {commandData.FirmwareId}.");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error occurred while trying to fetch a message from service bus.");
        }
        finally
        {
            // Close the sender and client
            await receiver.CloseAsync();
            await client.DisposeAsync();
        }
    }
}
