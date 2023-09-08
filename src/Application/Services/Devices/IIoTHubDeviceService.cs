namespace Application.Services.Devices;

public interface IIoTHubDeviceService
{
    Task RegisterDeviceToIotHub(Guid deviceId);
}
