using ApiShared.Dto.Time;

namespace ApiShared.Dto.Keys;

public class KeyResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string KeyUid { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }

    public DateTimeSpanResult Validity { get; init; } = null!;
}
