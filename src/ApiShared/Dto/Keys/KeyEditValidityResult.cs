using ApiShared.Dto.Time;

namespace ApiShared.Dto.Keys;

public class KeyEditValidityResult
{
    public Guid KeyId { get; init; }
    public DateTimeSpanResult Validity { get; init; } = null!;
}
