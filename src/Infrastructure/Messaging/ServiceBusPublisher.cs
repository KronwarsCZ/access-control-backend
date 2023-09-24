using Application.Services.Messaging;
using Azure.Messaging.ServiceBus;
using Serilog;

namespace Infrastructure.Messaging;

public class ServiceBusPublisher : IServiceBusPublisher
{
    private static string ConnectionString => Environment.GetEnvironmentVariable("SQLCONNSTR_azor");
    private const string QueueName = "firmware-update-queue";


    public async Task SendUpdateFirmwareMessage(FirmwareUpdateMessage payload)
    {
        await using var client = new ServiceBusClient(ConnectionString);
        var sender = client.CreateSender(QueueName);
        var message = new ServiceBusMessage(payload.ToString());

        try
        {
            await sender.SendMessageAsync(message);
            Log.Information($"Sent following message to service bus message: {payload}");
        }
        catch (Exception ex)
        {
            Log.Warning($"An error occurred while trying to send a message to service bus: {ex.Message}");
        }
        finally
        {
            // Close the sender and client
            await sender.CloseAsync();
            await client.DisposeAsync();
        }
    }
}
