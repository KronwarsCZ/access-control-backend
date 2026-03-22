namespace ApiShared.Dto.Devices;

public class DeviceRenameResult
{
    public Guid DeviceId { get; init; }
    public string Name { get; init; } = null!;
}
