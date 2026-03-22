namespace ApiShared.Dto.Devices;

public class DeviceResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? HardwareId { get; init; }
    public string Type { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
}
