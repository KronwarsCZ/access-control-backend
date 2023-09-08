namespace Application.Services.Devices;

public interface IIoTHubDeviceService
{
    Task RegisterDevice(Guid deviceId);
}
